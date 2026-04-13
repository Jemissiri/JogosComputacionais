/** @author hdaniel@ualg.pt
 *  @version 202603262000
 */
public abstract class Player : IPlayer  {
    
    protected string _name;
    protected int _position;

    public Player(string name, int position) { 
        this._name = name; 
        this._position = position;
    }

    public abstract int play(IBoard board);
    public string name() { return _name; }
    public int position() { return _position; }
    public void position(int pos) { _position = pos; }
    
}
