using System;

/*try
{*/
    using var game = new GrapplingGame.GameManager();
    game.Run();
/*}
catch(Exception e)
{
    System.IO.File.WriteAllText("log.txt", e.Message + "\n" + e.StackTrace);
}*/