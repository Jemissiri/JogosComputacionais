/** @author hdaniel@ualg.pt
 *  @version 202603262000
 */
public interface IPlayer {
    
    public const string expValidAction = "Not a valid action";

    /// <summary>
    /// Returns a valid action to play on the given board
    /// i.e., it must be one of the actions that leads to the 
    /// transition to one of the states returned by board.children()
    /// 
    /// If there are no pssible actions, it throws 
    /// Board.expNoEmptyCells exception 
    /// </summary>
    /// <param name="board">the board from where an action is to be played</param>
    /// <returns></returns>
    public int play(IBoard board);

    // returns the position of the player in the game
    // 0 for the first player, 1 for the second, etc.
    public int position();
    
    public void position(int pos);
    public string name();
}
