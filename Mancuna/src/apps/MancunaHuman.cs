/** @author hdaniel@ualg.pt
 *  @version 202604071200
 *
 *  Mancuna is a Mancala type game proposed and developed in Atari Basic by:   
 *
 *  @ see David Reeves (1986). Mancuna. 
 *        Atari User Vol.1 N0.12, pp. 31-33. 
 *        https://www.atarimania.com/mags/pdf/Atari-User-Vol-1-No-12.pdf]
 */
public class MancunaHuman
{
    /* Players have a second argument to define the position in the board (0 or 1):

        player 1, plays only from the top row
        
        (1) (0) possible actions for player 1  
        
         2 | 2
        -------
         2 | 2

        (0) (1) possible actions for player 0 

        player 0, plays only from the bottom row
    */
    static IPlayer rp = new PlayerMancRandom("Rnd", 0); 
    static IPlayer xp = new PlayerMancExpert("Expert", 0);
    static IPlayer hp = new PlayerMancHuman("Human", 0); 

    static List<IPlayer> players = [hp, rp];

    public static void Main2(string[] args)
    {
        BoardMancuna board = new(players);
        
        Match match = new Match(board, true);
        Score score = match.play();
                    
        if (score.winner() == -1) Console.WriteLine("It's a tie!");
        else Console.WriteLine("Winner: " + players[score.winner()].name());
    }

}