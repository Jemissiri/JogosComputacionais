/** @author hdaniel@ualg.pt
*  @version 202604011800
 *
 */

public class PlayerMancRandom : Player {
	protected Random rnd;

	public PlayerMancRandom(string name, int pos) : base(name, pos) { rnd = new Random(); }
	//To be reproducible, set the random seed
	public PlayerMancRandom(string name, int pos, int seed) : base(name, pos) { rnd = new Random(seed); }

	public override int play(IBoard board) { 
      	int forcedAction = board.forcedPlay(); 
      	if (forcedAction != -1) return forcedAction;

		return rnd.Next(0, 2);	}
}
