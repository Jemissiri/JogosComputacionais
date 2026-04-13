/** @author ?????
 *  @version ?????
 *
 *  Your GOAL is to develop this player to try 
 *  to beat the Expert players in MancunaMatch.
 */

public class PlayerMancuna : Player {

	public PlayerMancuna(string name, int pos) : base(name, pos) {  }

	public override int play(IBoard board) { 
      	
		if(board.isValidAction(0)) return 0;
		if(board.isValidAction(1)) return 1;
		
		//Should never get here
		throw new Exception("No valid action available");
	}
}
