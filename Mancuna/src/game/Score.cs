/** @author hdaniel@ualg.pt
 *  @version 202603262000
 */

public class Score {
    protected int _winner; //the player that won or -1 if it was a tie
    protected List<double> _score; //score of each player
    protected List<int> _moves; //number of moves for each player

    public Score(int winner, List<double> score, List<int> moves) { 
        _winner = winner;
        _score = score;
        _moves = moves;
    }

    public int winner() { return _winner; }
    public double score(int player) { return _score[player]; }
    public int Count { get { return _score.Count; } }
    public int moves(int player) { return _moves[player]; }

    public override string ToString() {
        string s = "winner = " + _winner + Environment.NewLine;
        for (int i=0; i<_score.Count; i++)
            s += "Player " + i + ": " + _score[i] + Environment.NewLine;
        return s.TrimEnd(Environment.NewLine.ToCharArray());
    }       
}