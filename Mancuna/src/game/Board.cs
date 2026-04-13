/** @author hdaniel@ualg.pt
 *  @version 202603262000
 */

public abstract class Board : IBoard
{
    protected int _startPlayer; //player 0 starts
    protected int _player = 0; //player 0 starts
    protected List<IPlayer> _players;
    
    protected Board(List<IPlayer> players) : this(players, 0) { }
    protected Board(List<IPlayer> players, int startPlayer) { 
        this._players = players; 
        this._startPlayer = startPlayer; 
        this._player = startPlayer;
        }

    public virtual void reset() { 
      _player = _startPlayer;
      for (int i=0; i<_players.Count; i++) _players[i].position(i);
    }

    public abstract void play(int action);
    public List<IPlayer> players() { return _players; }
    public void nextPlayer() { _player = (_player + 1) % _players.Count; }

    public abstract double score(int player);
    public abstract int winner();

    public abstract bool hasAction();
    public abstract bool isValidAction(int action);
    public abstract int firstAction();
    public abstract List<int> validActions();
    
    /// <summary>
    /// Check if only one action is available and return it
    /// </summary>
    /// <returns>Returns -1 if more than one action is available
    /// otherwise, it returns the only available action</returns>
    /// 
    /// default implementation, more than one action available
    
    public virtual int forcedPlay() { return -1; } 
    
    public abstract (List<IBoard>, List<int>) children();

    /// <summary>
    /// hash based on the layout of the board only
    /// </summary>
    /// <returns>the hash code</returns>
    public abstract int hash();

    public override bool Equals(object? obj) {
        if (obj == this) return true;
        if (obj == null) return false;
     
        // check if the start players are the same  
        if (_startPlayer != ((Board)obj)._startPlayer) return false; 

        // check if the players are the same  
        if (_player != ((Board)obj)._player) return false; 

        // check if the player list is the same reference
        if (_players != ((Board)obj)._players) return false; 

        return true;
    }


    /// <summary>
    /// Hash based on all instance variable
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() {
        // Compute hash code based on the layout data
        // this is slow, but there is another way?
        int _hash = 17;
        _hash += _player.GetHashCode();
        _hash += _players.GetHashCode();
        return _hash;
    }
            

}
