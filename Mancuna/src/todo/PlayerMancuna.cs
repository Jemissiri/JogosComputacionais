using System;
using System.Collections.Generic;
using System.Linq;

/** @author a75545 a79828 a79845 
 *
 *  MiniMax Agent for Mancuna with Alpha-Beta Pruning and Transposition Table.
 */

public class aa : Player {

    private enum Bound { Exact, Lower, Upper } // Lower comes from a beta cut, Upper from an alpha cut

    private struct CacheEntry {
        public double value;
        public int depth;
        public Bound bound;
    }

    private Dictionary<int, CacheEntry> cache = new();

	public aa(string name, int pos) : base(name, pos) {  }

	public override int play(IBoard board) { 
        cache.Clear();

        int forcedAction = board.forcedPlay(); 
        if (forcedAction != -1) return forcedAction;

        var (children, actions) = board.children();
        if (actions.Count == 0) return board.firstAction();

        // --- Move Ordering ---
        // Sort actions by their immediate heuristic value to improve pruning
        var ordered = actions.Select((action, index) => new { 
            Action = action, 
            Child = children[index], 
            Score = Heuristic(children[index]) 
        }).OrderByDescending(x => x.Score).ToList();

        int bestAction = ordered[0].Action;
        double bestValue = double.NegativeInfinity;

	    int searchDepth = 12; 

        foreach (var item in ordered) {
            double value = Minimax(item.Child, searchDepth, double.NegativeInfinity, double.PositiveInfinity, false);

            if (value > bestValue) {
                bestValue = value;
                bestAction = item.Action;
            }
        }

	    return bestAction;
	}
    private double Minimax(IBoard board, int depth, double alpha, double beta, bool maximizingPlayer) {
        int winner = board.winner();
        if (winner != (int)GameEnd.InProgress) {
            if (winner == _position) return 1000 + depth;
            if (winner == (int)GameEnd.Tie) return board.score(_position) - board.score(1 - _position);
            return -1000 - depth;
        }

        if (depth == 0) return Heuristic(board);

        int stateHash = board.GetHashCode();
        if (cache.TryGetValue(stateHash, out CacheEntry entry) && entry.depth >= depth) {
            if (entry.bound == Bound.Exact) return entry.value;
            if (entry.bound == Bound.Lower && entry.value >= beta) return entry.value;
            if (entry.bound == Bound.Upper && entry.value <= alpha) return entry.value;
        }

        var (children, actions) = board.children();
        if (actions.Count == 0) return Heuristic(board);

        double alphaOriginal = alpha;
        double betaOriginal = beta;
        double evalValue;

        if (maximizingPlayer) {
            double maxEval = double.NegativeInfinity;
            for (int i = 0; i < children.Count; i++) {
                double eval = Minimax(children[i], depth - 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            evalValue = maxEval;
        } else {
            double minEval = double.PositiveInfinity;
            for (int i = 0; i < children.Count; i++) {
                double eval = Minimax(children[i], depth - 1, alpha, beta, true);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha) break;
            }
            evalValue = minEval;
        }

        CacheEntry newEntry = new CacheEntry { value = evalValue, depth = depth };
        if (evalValue <= alphaOriginal) newEntry.bound = Bound.Upper;
        else if (evalValue >= betaOriginal) newEntry.bound = Bound.Lower;
        else newEntry.bound = Bound.Exact;
        
        cache[stateHash] = newEntry;

        return evalValue;
    }

    // here this small weight in the (board.validActions().Count * 0.1) encourages the agent to keep tokens distributed across its cells
    // maintaining flexibility and preventing a sudden "empty side" loss.
    private double Heuristic(IBoard board) {
        int opponent = 1 - _position;
        return board.score(_position) - board.score(opponent) + board.validActions().Count * 0.1;
    }
}
