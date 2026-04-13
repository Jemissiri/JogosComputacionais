/** @author hdaniel@ualg.pt
 *  @version 202604071200
 *
 *  Mancuna is a Mancala type game proposed and developed in Atari Basic by:   
 *
 *  @ see David Reeves (1986). Mancuna. 
 *        Atari User Vol.1 N0.12, pp. 31-33. 
 *        https://www.atarimania.com/mags/pdf/Atari-User-Vol-1-No-12.pdf]
 */
public class MancunaMatch
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
    static IPlayer xp = new PlayerMancExpert("Expert", 0);  // the ones we have to try to beat
    static IPlayer mp = new PlayerMancuna("MancunaPlayer0", 0);

    static int matches = 10000;
    static bool verbose = false;

    public static void Main(string[] args)
    {
        /*
            Choose players for position 0 and 1
            Players play in both positions to avoid bias of starting player
        */
        List<IPlayer>players = [mp, xp];

        int[,] w=new int[2,2];
        int[,] s=new int[2,2]; 
        int[,] m=new int[2,2];
        int[]  t=new int[2];    
        for (int j=0; j<2; j++) {    

            BoardMancuna board = new(players);
            Match match = new Match(board, verbose);

            for (int i=0; i<matches; i++) {    
                
                board.reset();          
                Score score = match.play();
                
                int winner = score.winner();
                if (winner == -1) t[j]++;
                else if (winner == 0) w[j,0]++;
                else if (winner == 1) w[j,1]++;
                s[j,0] += (int) score.score(0);
                s[j,1] += (int) score.score(1);
                m[j,0] += score.moves(0);
                m[j,1] += score.moves(1);
            }

            //Print partial results
            printResults(players, [w[j,0], w[j,1]], [s[j,0], s[j,1]], [m[j,0], m[j,1]], t[j]);

            //switch players for next match
            players.Reverse();
        }

        //Print results
        Console.WriteLine("\nFinal results after " + matches*2 + " matches:");
        printResults(players, [w[0,0]+w[1,1], w[0,1]+w[1,0]], [s[0,0]+s[1,1], s[0,1]+s[1,0]], [m[0,0]+m[1,1], m[0,1]+m[1,0]], t[0]+t[1]);
    }

    static void printResults(List<IPlayer> players, int[] w, int[] s, int[] m, int t) {
        Console.WriteLine($"Player 0: {players[0].name(),20} wins: {w[0],5} score: {s[0],7} moves: {m[0],7}");
        Console.WriteLine($"Player 1: {players[1].name(),20} wins: {w[1],5} score: {s[1],7} moves: {m[1],7}");
        Console.WriteLine("Ties: " + t);
    }

}

