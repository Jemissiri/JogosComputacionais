/** @author hdaniel@ualg.pt
 *  @version 202604041800
 *
 * This is a C# version (already trained) of the
 * AI for the Mancuna game developed in Atari Basic by:   
 *
 *  @ see David Reeves (1986). Mancuna. 
 *        Atari User Vol.1 N0.12, pp. 31-33. 
 *        https://www.atarimania.com/mags/pdf/Atari-User-Vol-1-No-12.pdf]
 */
public class PlayerMancExpert : Player {

    static	string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../src/data/qtable.dat");
		
    static protected readonly int totalPieces = 8;
    public static readonly int[] mult = {100, 10}; 
    static protected readonly int qualLen = totalPieces * mult[0] + 1;

    static public readonly char lossQuality    = '1';
    static public readonly char winQuality     = '2';
    private char[][] playQuality = [new char[qualLen], new char[qualLen]];
    


    public PlayerMancExpert(string name, int pos) : base(name, pos)
    {
        load(basePath);
    }

    
    // load the play quality maps from a file
    protected void load(string path) {
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            if (lines.Length >= 2)
            {
                playQuality[0] = lines[0].ToCharArray();
                playQuality[1] = lines[1].ToCharArray();
            }
        }
    }


    protected int address(BoardMancuna board) {
        int a = board.data[0];
        int b = board.data[1];
        int c = board.data[2];
        return mult[0]*a+mult[1]*b+c;
    }

    
    public override int play(IBoard board) { 
        
        // if only one action is available, play it
        int action = board.forcedPlay(); 
        if (action != -1) return action;

        // check possible moves quality
        else {
            BoardMancuna b0 = new BoardMancuna((BoardMancuna)board);
            BoardMancuna b1 = new BoardMancuna((BoardMancuna)board);
            b0.play(0);
            b1.play(1);

            char qual0 = playQuality[_position][address(b0)];
            char qual1 = playQuality[_position][address(b1)];

            if      (qual0 == winQuality)  return 0;
            else if (qual1 == winQuality)  return 1;
            else if (qual0 == lossQuality) return 1;
            else if (qual1 == lossQuality) return 0;
            else                           return 1;  
        }
    }

}
