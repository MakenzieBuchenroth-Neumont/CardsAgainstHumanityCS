██████╗ ██╗ ██████╗ ██╗████████╗ █████╗ ██╗                        
██╔══██╗██║██╔════╝ ██║╚══██╔══╝██╔══██╗██║                        
██║  ██║██║██║  ███╗██║   ██║   ███████║██║                        
██║  ██║██║██║   ██║██║   ██║   ██╔══██║██║                        
██████╔╝██║╚██████╔╝██║   ██║   ██║  ██║███████╗                   
╚═════╝ ╚═╝ ╚═════╝ ╚═╝   ╚═╝   ╚═╝  ╚═╝╚══════╝                   
                                                                   
 ██████╗ █████╗ ██████╗ ██████╗ ███████╗                           
██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔════╝                           
██║     ███████║██████╔╝██║  ██║███████╗                           
██║     ██╔══██║██╔══██╗██║  ██║╚════██║                           
╚██████╗██║  ██║██║  ██║██████╔╝███████║                           
 ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚══════╝                           
                                                                   
 █████╗  ██████╗  █████╗ ██╗███╗   ██╗███████╗████████╗            
██╔══██╗██╔════╝ ██╔══██╗██║████╗  ██║██╔════╝╚══██╔══╝            
███████║██║  ███╗███████║██║██╔██╗ ██║███████╗   ██║               
██╔══██║██║   ██║██╔══██║██║██║╚██╗██║╚════██║   ██║               
██║  ██║╚██████╔╝██║  ██║██║██║ ╚████║███████║   ██║               
╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝               
                                                                   
██╗  ██╗██╗   ██╗███╗   ███╗ █████╗ ███╗   ██╗██╗████████╗██╗   ██╗
██║  ██║██║   ██║████╗ ████║██╔══██╗████╗  ██║██║╚══██╔══╝╚██╗ ██╔╝
███████║██║   ██║██╔████╔██║███████║██╔██╗ ██║██║   ██║    ╚████╔╝ 
██╔══██║██║   ██║██║╚██╔╝██║██╔══██║██║╚██╗██║██║   ██║     ╚██╔╝  
██║  ██║╚██████╔╝██║ ╚═╝ ██║██║  ██║██║ ╚████║██║   ██║      ██║   
╚═╝  ╚═╝ ╚═════╝ ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝   ╚═╝      ╚═╝ 


This solution contains 3 projects:

  1. CardsAgainstHumanity - this is the console application version of the client
  2. CardsAgainstHumanityGUI - this is the WPF gui version of the client
  3. Server - this is the server and is a console application.

All testing was done on a local lan over wired ethernet and wirelessly using a phone as a hotspot.
It does work over WAN but the port 1337 needs to be forwarded to the server for this to work (WAN play has not been tested so could unexpected behaviour).

To Play:

  1. start the server and choose a standard game or a custom game.
  2. launch the client of your choice.
  3. optionally add ai's to the game with the command "!game.addAI" from the server console //their must be at least 3 players to have a functioning game, 1 human player can play with any number of ai's but will always be the card czar
  4. when ready you can start the game from the server console with "!game.start" // all commands can be viewed by doing "!help" from the server console
  5. have fun, this game is not for the easily offeneded!

Code can be viewed on github at : https://github.com/ThatCrazyIrishGuy/CardsAgainstHumanityCS