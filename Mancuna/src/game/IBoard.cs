/** @author hdaniel@ualg.pt
 *  @version 202603262000
 */

public interface IBoard
{
    /// <summary>
    /// resets the board
    /// </summary>
    public void reset();

    /// <summary>
    /// updates the board with the action specified, if valid
    /// </summary>
    /// <param name="action"></param>
    public void play(int action);


    public List<IPlayer> players();

    /// <summary>
    /// specifies that the next move will be played by the next player
    /// </summary>
    public void nextPlayer();
    
    /// <summary>
    /// returns the score of the player specified
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public double score(int player);

    /// <summary>
    /// Returns the number of the player that won [0 .. players.Count-1] 
    /// or -1 if it was a tie  
    /// or -2 if the game is not finished yet
    /// </summary>
    /// <returns></returns>
    public int winner();

    /// <summary>
    /// Returns true if at least there is one action available to be played
    /// </summary>
    /// <returns></returns>
    public bool hasAction();

    /// <summary>
    /// Checks if the action specified is valid
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool isValidAction(int action);

    /// <summary>
    /// returns the first action in the list returned by validActions()  or -1 if none
    /// </summary>
    /// <returns></returns>
    public int firstAction();

    /// <summary>
    /// returns a list with all the valid actions
    /// </summary>
    /// <returns></returns>
    public List<int> validActions();

    /// <summary>
    /// Check if only one action is available and return it
    /// </summary>
    /// <returns>Returns -1 if more than one action is available
    /// otherwise, it returns the only available action</returns>
    public int forcedPlay();

    /// <summary>
    /// returns all the possible successors of this board
    /// </summary>
    /// <param name="children("></param>
    /// <returns></returns>
    public (List<IBoard>, List<int>) children();

    /// <summary>
    /// hash based on the layout of the board only
    /// </summary>
    /// <returns>the hash code</returns>
    public int hash();

}
    