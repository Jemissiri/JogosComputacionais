

using System.ComponentModel.Design;


/** @author hdaniel@ualg.pt
 *  @version 202604011800
 *
 */


public class PlayerMancHuman : Player {

    public PlayerMancHuman(string name, int position) 
      : base(name, position) { }


    public override int play(IBoard board)
    {
        int action=-1;

        Console.Write("Play left or right square? press (l or r) ");
        while (true) {

            string? input = Console.ReadLine();

            if (input == "l" || input == "L") action = 0;   
            else if (input == "r" || input == "R") action = 1; 
            else {
                Console.Write("Invalid input! press (l or r) ");
                continue;
            }
            
            if (!board.isValidAction(action)) 
                Console.Write("No counters on this square! Must play the other ");
            else  
                break;    
        }
            
        return action;
    }
}

