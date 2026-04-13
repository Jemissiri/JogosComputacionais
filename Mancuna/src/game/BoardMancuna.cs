/** @author hdaniel@ualg.pt
 *  @version 202603181400
 */
using System.Text;

public class BoardMancuna : Board {
    
    public const string expInvalidNumberPlayers = "Invalid number of players: must have exactly 2 players";
    public const string expInvalidLayoutNumber  = "Invalid layout: must contain exactly 4 digits between 0 and 8";
    public const string expInvalidLayoutSum     = "Invalid layout: sum of the 4 digists must be 8";

    protected static readonly int resetCellValue = 2;
    protected static readonly int numPieces = 8;
    public static readonly int nrows=2;
	public static readonly int ncols=2;
    protected static readonly int length = nrows * ncols;
    
    protected const int EmptyCell = 0;
        
    // 2D array to represent the 2x2 layout of the board
    public int[] data = new int[nrows * ncols]; 
	    
    protected int repsToTie;
    protected bool tied = false; // to check tie after 3 repeated positions
    protected Dictionary<int, int> playerBoards=new();

    public BoardMancuna(List<IPlayer> players) : this([2,2,2,2], players, 0) { }

    public BoardMancuna(int[] layout, List<IPlayer> players, int startPlayer=0, int repsToTie=3) 
        : base(players, startPlayer) {

        if (players.Count != 2)
            throw new ArgumentException(expInvalidNumberPlayers);

        if (layout.Length != length)
            throw new ArgumentException(expInvalidLayoutNumber);
        if (layout.Sum() != 8)
            throw new ArgumentException(expInvalidLayoutSum);

        this.repsToTie = repsToTie;
        Array.Copy(layout, data, length);
        }
	

    //Copy constructor
    //It is a shallow copy, i.e., the players list is not copied, 
    //only the reference to it is copied
    public BoardMancuna(BoardMancuna b) : base(b._players, b._startPlayer) {         
        // copy the layout of the board 
        _player = b._player;
        Array.Copy(b.data, data, b.data.Length);
    }
    

    public override void reset() {
        base.reset();
        Array.Fill(data, resetCellValue);
        playerBoards.Clear();
        tied = false;
    }


    public override (List<IBoard>, List<int>) children() {
        List<IBoard> childList = new(2);
        List<int> actions = new(2);

        // faster than calling isValidAction twice
        int idx = _player*2;
        if (data[idx] != EmptyCell) {
        //if (isValidAction(0)) { 
            BoardMancuna child = new(this);
            child.play(0);
            child.nextPlayer(); // switch to the other player
            childList.Add(child);
            actions.Add(0);
        }

        if (data[idx+1] != EmptyCell) {
        //if (isValidAction(1)) { 
            BoardMancuna child = new(this);
            child.play(1);
            child.nextPlayer(); // switch to the other player
            childList.Add(child);
            actions.Add(1);
        }
        
		return (childList, actions);
    }


    /// <summary>
    /// returns true if there is no more actions
    /// </summary>
    /// <returns></returns>
    public override bool hasAction() {
        // tied after 3 repeated positions
        return !tied;
    }


    //PRE: action in { 0, 1 }
    public override bool isValidAction(int action) {
        int idx = action + _player*2;
        return data[idx] != EmptyCell;
    }


    public override int firstAction() {
        //if (isValidAction(0)) return 0;
        //if (isValidAction(1)) return 1;

        // faster than calling isValidAction twice
        int idx = _player*2;
        if (data[idx]   != EmptyCell) return 0;
        if (data[idx+1] != EmptyCell) return 1;
        return -1; //all cells empty
    }


    public override List<int> validActions() {
		List<int> valid = new();
		//if (isValidAction(0)) valid.Add(0);
        //if (isValidAction(1)) valid.Add(1);

        // faster than calling isValidAction twice
        int idx = _player*2;
        if (data[idx]   != EmptyCell) valid.Add(0);
        if (data[idx+1] != EmptyCell) valid.Add(1);

        return valid;
    }


    /// <summary>
    /// Check if only one action is available and return it
    /// </summary>
    /// <returns>Returns -1 if both actions are available
    /// otherwise, it returns the only available action</returns>
    public override int forcedPlay() {
        // faster than calling isValidAction twice
        //int idx = _player*2;
        //bool v0 = data[idx]    != EmptyCell;
        //bool v1 = (data[idx+1] != EmptyCell);
        bool v0 = this.isValidAction(0);
        bool v1 = this.isValidAction(1);

        if (!v0 && !v1) 
            throw new InvalidOperationException(IPlayer.expValidAction);
        
        if (!v0) return 1;
        if (!v1) return 0;
        return -1;
    }


    // PRE: action in { 0, 1 } && isValidAction(action)
    public override void play(int action) {       
        int idx = action + _player*2;
        int tokens = data[idx];
        data[idx] = 0;
        
        for (int i=0; i<tokens; i++) {
            idx = (idx + 1) % length;
            data[idx]++;
        }

        // add to playedBoards to check tie after 3 repeated positions
        int _hash = hash();
        if (playerBoards.ContainsKey(_hash)) {
            playerBoards[_hash]++;
            if (playerBoards[_hash] == repsToTie) tied = true;
        }
        else
            playerBoards[_hash] = 1;
    }


    public override int winner()
    {
        if (data[0] + data[1] == 0) return 1;
        if (data[2] + data[3] == 0) return 0;
        
        if (!hasAction())return (int) GameEnd.Tie; 
        return (int) GameEnd.InProgress;    
    }


    /// <summary>
    /// returns the sum of tokens in the player's 2 cells
    /// </summary>
    /// <param name="player">the player for whom to calculate the score</param>
    /// <returns>the score for the given player</returns>
    public override double score(int player)
    {
        int idx = player*2;
        return data[idx] + data[idx + 1];
    }


    public override bool Equals(object? obj) {
        if (!(obj is BoardMancuna))   return false;
        
        //compare base class instance  variables
        if (!base.Equals((Board)obj)) return false;
        
        // compare board layout
        for(int i=0; i<length; i++)
            if (data[i] != ((BoardMancuna)obj).data[i])
                return false;

		return true;
    }


    /// <summary>
    /// hash based on the layout of the board 
    /// </summary>
    /// <returns>the hash code</returns>
    public override int hash()
    {
        return data[0]*1000 + data[1]*100 + data[2]*10 + data[3];
    }

    /// <summary>
    /// Hash based on all instance variable
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() {
        return hash() + base.GetHashCode();
    }


    public override string ToString() {
        return ""+data[3]+data[2]+"\n"+data[0]+data[1];
    }

}