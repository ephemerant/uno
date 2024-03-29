We would like to implement a multiplayer “UNO” card game in C#. We will use TCP sockets to allow for both LAN and internet games, with a single client choosing to be the host. Each game will consist of 2-10 players at a time. We will handle cases of players disconnecting from the game, both client and host. We may also implement a basic chat room to allow for playful banter.

As this is a networking-centric class, we will focus on packet security and verification, and we may also look into preventing the host from using a memory editor to benefit themselves. Beyond the networking and security aspects, we hope to implement smooth, responsive graphics. We also would like to implement basic computer opponents, depending on how much time we have left over.

A URL to an example of a basic “Tic Tac Toe” game that uses TCP sockets can be found below, which we will study and use as a basic reference model, although our end result will greatly surpass it in scope:

http://www.codeproject.com/Articles/6574/C-TicTacToe-with-AI-and-network-support
