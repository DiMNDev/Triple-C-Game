namespace Chess_Final.Tests;
using Player;
using FluentAssertions;
using Chess_Final.Generics;
using Chess_Final.Chess;
using Chess_Final.Lobby;
using TC_DataManagerException;
using TC_DataManager;
using Chess_Final.PlayerManager;
using Chess_Final.DB_Manager;

public class Player_Tests
{
    [Fact]
    public void ShouldCreateNewPlayer()
    {
        // Arrange
        // Act
        Player player = new Player("Jane");

        // Assert
        player.Should().NotBeNull();
    }
    [Fact]
    public void ShouldHaveCorrectPlayerName()
    {
        // Arrange
        var newPlayerName = "John";
        // Act
        Player player = new Player(newPlayerName);

        // Assert
        player.Username.Should().Be(newPlayerName);
    }

    [Fact]
    public void PlayerShouldHaveGuid()
    {
        // Arrange
        var newPlayerName = "John";
        // Act
        Player player = new Player(newPlayerName);
        var id = player.PlayerID;

        // Assert
        player.PlayerID.Should().Be(id);
        player.PlayerID.Should().NotBeEmpty();
    }



}

public class GamePiece_Tests
{
    public class Pawn_Tests
    {
        [Fact]
        public void ShouldHaveValidMove()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();
            // Act
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOnePawn.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOnePawn.CurrentPosition.Y;
            var PlayerOneFirstMove = (0, 4);
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.NewTurn();

            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoPawn.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoPawn.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);
            var PlayerTwoFirstMove = (0, 3);
            // Assert
            PlayerOnePawn!.AllowedMovement.Count().Should().Be(2);
            PlayerOnePawn!.AllowedMovement[0].Should().Be(PlayerOneFirstMove);
            PlayerTwoPawn!.AllowedMovement.Count().Should().Be(2);
            PlayerTwoPawn!.AllowedMovement[0].Should().Be(PlayerTwoFirstMove);

        }
        [Fact]
        public void ShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOnePawn.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOnePawn.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 4, game);

            // Assert
            PlayerOnePawn.CurrentPosition.Should().Be(("A", 4));
            game.Board.Matrix[0, 4].Should().Be(PlayerOnePawn);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void ShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();

            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.Name == "Pawn").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOnePawn.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOnePawn.CurrentPosition.Y;

            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.Name == "Pawn").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoPawn.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoPawn.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 4, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 3, game);

            // Assert
            PlayerOnePawn.CurrentPosition.Should().Be(("A", 4));
            game.Board.Matrix[0, 4].Should().Be(PlayerOnePawn);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void PawnShouldAllowPawnToAttack()
        {
            // Arrange

            // Setup Game
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();

            // Setup PlayerOne
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("C", 6)).FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOnePawn.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOnePawn.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("B", 1)).FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoPawn.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoPawn.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(2, 4, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(1, 3, game);
            game.CurrentPlayer = PlayerOne;
            PlayerOne.Select(2, 4, game, PlayerOne);
            PlayerOne.MovePiece(1, 3, game);
            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoPawn) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOnePawn.CurrentPosition.Should().Be(("B", 3));
            game.Board.Matrix[1, 3].Should().Be(PlayerOnePawn);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class Rook_Tests
    {
        [Fact]
        public void RookShouldNotHaveAnyValidMoves()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();
            // Act
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneRook.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.NewTurn();

            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoRook.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoRook.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);
            // Assert
            PlayerOneRook!.AllowedMovement.Count().Should().Be(0);
            PlayerTwoRook!.AllowedMovement.Count().Should().Be(0);

        }
        [Fact]
        public void RookShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneRook.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 4, game);

            // Assert
            PlayerOneRook.CurrentPosition.Should().Be(("A", 4));
            game.Board.Matrix[0, 4].Should().Be(PlayerOneRook);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void RookShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();

            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneRook.CurrentPosition.Y;

            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoRook.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoRook.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 4, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 3, game);

            // Assert
            PlayerOneRook.CurrentPosition.Should().Be(("A", 4));
            game.Board.Matrix[0, 4].Should().Be(PlayerOneRook);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void RookShouldAllowRookToAttack()
        {
            // Arrange

            // Setup Game
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();

            // Setup PlayerOne
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneRook.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoRook.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoRook.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(2, 4, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(1, 3, game);
            game.CurrentPlayer = PlayerOne;
            PlayerOne.Select(2, 4, game, PlayerOne);
            PlayerOne.MovePiece(1, 3, game);
            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoRook) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneRook.CurrentPosition.Should().Be(("B", 3));
            game.Board.Matrix[1, 3].Should().Be(PlayerOneRook);
            RemovedFromPlay.Should().BeNull();
        }
    }

}


public class GameBoard_Tests
{
    [Fact]
    public void ChessBoardShouldBe8X8()
    {
        // Arrange
        // Act
        GameBoard gameBoard = new GameBoard(GameType.Chess);

        // Assert
        gameBoard.BoardSize.Should().Be((8, 8));
    }
}

public class ChessGame_Tests
{
    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerOneShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne);
        // Assert
        chess.PlayerOne.Username.Should().Be("John");
    }
    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerTwoShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        chess.JoinGame(playerOne);
        chess.JoinGame(playerTwo);
        // Assert
        chess.PlayerOne.Username.Should().Be("John");
        chess.PlayerTwo.Username.Should().Be("Jane");
    }

    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void ShouldAddSpectatorIfPlayerSeatsAreFull()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        Player SpectatorOne = new Player("Jack");
        chess.JoinGame(playerOne);
        chess.JoinGame(playerTwo);
        chess.JoinGame(SpectatorOne);
        // Assert
        chess.Spectators[0].Should().Be(SpectatorOne);

    }
    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerOnePieceShouldHaveCorrectPosition()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne);
        // Assert
        chess.PlayerOne.GamePieces[1].CurrentPosition.Should().Be((ChessCoordinate.B.ToString(), 6));
    }

    // Requires path: "FilePaths.Tests" in LayoutGamePieces 
    [Fact]
    public void PlayerTwoPieceShouldHaveCorrectPosition()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        Player playerTwo = new Player("Jane");
        chess.JoinGame(playerOne);
        chess.JoinGame(playerTwo);

        // Assert
        chess.PlayerTwo.GamePieces[0].CurrentPosition.Should().Be((ChessCoordinate.A.ToString(), 1));
    }

}

public class DataManager_Tests
{
    [Fact]
    public void LoadFileShouldNotThrowDataLoadErrorException()
    {
        // Arrange
        void Load()
        {
            DataManager.LoadFile<PlayerData>("path.json");
        }
        // Act
        FluentActions.Invoking(Load).Should().Throw<DataLoadErrorException>();
        // Assert
    }
}

public class Lobby_Tests
{
    [Fact]
    public void ShouldHaveAListOfGamesForEachGameType()
    {
        // Arrange

        // Act

        // Assert

    }
    [Fact]
    public void ShouldFilterActiveGamesWithCorrectGuid()
    {
        // Arrange

        // Act

        // Assert

    }
    [Fact]
    public void ShouldFilterOpenGamesWithCorrectGuid()
    {
        // Arrange
        Player playerOne = new("P1");
        Player playerTwo = new("P2");
        Player playerThree = new("P3");
        int previousGameCount = LobbyManager.ChessGames.Select(d => d.Value.Open == true).ToList().Count();
        Guid g1 = LobbyManager.CreateGame(GameType.Chess);
        Guid g2 = LobbyManager.CreateGame(GameType.Chess);
        Game gameOne = LobbyManager.GetGame(GameType.Chess, g1);
        Game gameTwo = LobbyManager.GetGame(GameType.Chess, g2);
        gameOne.JoinGame(playerOne);
        gameOne.JoinGame(playerTwo);
        gameTwo.JoinGame(playerThree);
        // Act
        var OpenGames = LobbyManager.FilterByOpen(GameType.Chess);

        // Assert
        OpenGames.ContainsKey(gameTwo.UUID).Should().BeTrue();
        OpenGames.Count().Should().Be(previousGameCount + 1);
    }
    [Fact]
    public void ShouldAddANewGameToLobby()
    {
        // Arrange

        Player player = new Player("John");
        Guid newGameGuid = LobbyManager.CreateGame(GameType.Chess);
        // Act
        // Assert
        LobbyManager.ChessGames.FirstOrDefault(g => g.Key == newGameGuid).Should().NotBeNull();
    }
}

public class DB_Tests
{
    [Fact]
    public void ShouldInsertANewRecordAndReturnTrue()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        string username = "Test";
        string password = "SooperSecret";
        // Act
        Player TestPlayer = new(username);
        bool result = dB_Connect.InsertRecord(TestPlayer, password);
        // Assert
        result.Should().BeTrue();
    }
    [Fact]
    public void ShouldReturnTrueIfUserExists()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        // Act
        bool UserExists = dB_Connect.AccountExists("Test");
        // Assert
        UserExists.Should().BeTrue();
    }
    [Fact]
    public void ShouldDeleteAnExistingRecord()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        string username = "Test";
        // Act
        dB_Connect.DeleteRecord(username);
        bool StillExists = dB_Connect.AccountExists(username);
        // Assert
        StillExists.Should().BeFalse();
    }
    [Fact]
    public void ShouldReturnFalseIfUserExists()
    {
        // Arrange
        DB_Connect dB_Connect = new();
        // Act
        bool UserExists = dB_Connect.AccountExists("Test1");
        // Assert
        UserExists.Should().BeFalse();
    }

}
public class Login_Tests
{
    [Fact]
    public void ShouldCreateAnAccountIfUsernameIsNotInUse()
    {
        // Arrange
        string username = "username";
        string password = "soopersecure";
        string confirm = "soopersecure";
        DB_Connect dB_Connect = new();
        if (dB_Connect.AccountExists(username))
        {
            dB_Connect.DeleteRecord(username);
        }
        // Act
        Player player = PlayerManager.SignUp(username, password, confirm);
        // Assert
        player.Username.Should().Be(username);
    }
    [Fact]
    public void ShouldSignIntoAccountIfUsernameAndPasswordCombinationMatch()
    {
        // Arrange
        string username = "username";
        string password = "soopersecure";
        // Act
        var result = PlayerManager.SignIn(username, password);
        // Assert
        result.Should().NotBeNull();
    }
    [Fact]
    public void ShouldAddPlayerToListOfOnlinePlayers()
    {
        // Arrange
        string username = "username";
        string password = "soopersecure";
        // Act
        var result = PlayerManager.SignIn(username, password);
        // Assert
        result.Should().NotBeNull();
    }

}