/** @author hdaniel@ualg.pt
 *  @version 202603262000
 */
 
public class Match {

    protected List<IPlayer> players;
    protected List<int> moves;
    protected IBoard board;
    protected bool verbose;

    public Match(IBoard board, bool verbose=false) {
        this.players = board.players();
        this.board = board;
        this.verbose = verbose;
        this.moves = [.. new int[players.Count]];
    }

    /// <summary>
    /// plays the game calling each player to play, in order, until the game is over.
    /// The game is over when the board is in a goal state, which can be checked with the method
    /// board.isGoal() (or similar). The
    /// </summary>
    /// <returns>-1 if it was a tie or the number of the player that won [0 .. players.Count-1]</returns>
    public Score play() {
        int winner = (int) GameEnd.InProgress;
        for (int i=0; i<players.Count; i++) moves[i] = 0;
        
        if (verbose) printBoard(board);
        while (winner == (int) GameEnd.InProgress) {
            for (int i=0; i<players.Count; i++) {
                
                moves[i]++;
                IPlayer p = players[i];

                int action = p.play(board);
                if (!board.isValidAction(action)) 
                    throw new InvalidOperationException(IPlayer.expValidAction);  
                board.play(action);         

                if (verbose) printBoard(board);

                board.nextPlayer();
                winner = board.winner();
                if (winner != (int) GameEnd.InProgress)                    
                    break;                    
                }
            }
        
        //Return winners and scores 
        List<double> scoreList = new();
        for (int i=0; i<players.Count; i++)
            scoreList.Add(board.score(i));
        
        return new(winner, scoreList, moves);
    }

    void printBoard(IBoard board)
    {
        Console.WriteLine(players[1].name());
        Console.WriteLine(board);
        Console.WriteLine(players[0].name());
        Console.WriteLine("");
    }
}