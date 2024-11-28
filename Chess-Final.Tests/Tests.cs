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
            PlayerTwoPawn.CurrentPosition.Should().Be(("A", 3));
            game.Board.Matrix[0, 3].Should().Be(PlayerTwoPawn);
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
        public void RookShouldNotHaveAnyValidMovesForBothPlayers()
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
        public void RookShouldHaveMultipleValidMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            // Act
            PlayerOneRook.CurrentPosition = ("C", 3);
            game.PlaceInMatrix();
            Enum.TryParse<ChessCoordinate>(PlayerOneRook.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneRook.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);
            // Assert
            PlayerOneRook.AllowedMovement.Count().Should().Be(11);

        }
        [Fact]
        public void RookShouldHaveMultipleValidMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook").FirstOrDefault();
            // Act
            PlayerTwoRook.CurrentPosition = ("C", 3);
            game.PlaceInMatrix();
            Enum.TryParse<ChessCoordinate>(PlayerTwoRook.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerTwoRook.CurrentPosition.Y;
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerTwo);
            // Assert
            PlayerTwoRook.AllowedMovement.Count().Should().Be(11);

        }
        // Unfinished
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
    public class Knight_Tests
    {
        [Fact]
        public void KnightShouldHaveTwoMovesForBothPlayers()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            game.PlaceInMatrix();
            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKnight.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneKnight.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoKnight.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKnight.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);
            // Assert
            PlayerOneKnight.AllowedMovement.Count().Should().Be(2);
            PlayerTwoKnight.AllowedMovement.Count().Should().Be(2);

        }
        [Fact]
        public void KnightShouldHaveEightValidMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            // Act
            PlayerOneKnight.CurrentPosition = ("D", 3);
            game.PlaceInMatrix();
            Enum.TryParse<ChessCoordinate>(PlayerOneKnight.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneKnight.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);
            // Assert
            PlayerOneKnight.AllowedMovement.Count().Should().Be(8);

        }
        [Fact]
        public void KnightShouldHaveEightValidMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            // Act
            PlayerTwoKnight.CurrentPosition = ("D", 4);
            game.PlaceInMatrix();
            Enum.TryParse<ChessCoordinate>(PlayerTwoKnight.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerTwoKnight.CurrentPosition.Y;
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerTwo);
            // Assert
            PlayerTwoKnight.AllowedMovement.Count().Should().Be(8);

        }
        // Unfinished
        [Fact]
        public void KnightShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneKnight.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneKnight.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);

            // Assert
            PlayerOneKnight.CurrentPosition.Should().Be(("A", 5));
            game.Board.Matrix[0, 5].Should().Be(PlayerOneKnight);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void KnightShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();

            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneKnight.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneKnight.CurrentPosition.Y;

            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoKnight.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKnight.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 2, game);

            // Assert
            PlayerTwoKnight.CurrentPosition.Should().Be(("A", 2));
            game.Board.Matrix[0, 2].Should().Be(PlayerTwoKnight);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void KnightShouldAllowKnightToAttack()
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
            GamePiece? PlayerOneKnight = PlayerOne!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneKnight.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneKnight.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoKnight = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoKnight.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKnight.CurrentPosition.Y;

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
                    if (game.Board.Matrix[i, j] == PlayerTwoKnight) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneKnight.CurrentPosition.Should().Be(("B", 3));
            game.Board.Matrix[1, 3].Should().Be(PlayerOneKnight);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class Bishop_Tests
    {
        [Fact]
        public void BishopShouldHaveFiveMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 7)).FirstOrDefault();
            // Remove pawn @ (D,6) -> (D,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("D", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneBishop.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneBishop.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            // Assert
            PlayerOneBishop.AllowedMovement.Count().Should().Be(5);
        }
        [Fact]
        public void BishopShouldHaveFiveMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();

            // Move pawn @ (D,1) -> (D,2)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("D", 2);
            game.PlaceInMatrix();
            // Act            
            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoBishop.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoBishop.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerTwoBishop.AllowedMovement.Count().Should().Be(5);

        }
        [Fact]
        public void BishopShouldHaveSevenValidMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 7)).FirstOrDefault();
            // Move pawn @ (D,6) -> (D,5)
            GamePiece? PlayerOnePawnOne = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawnOne.CurrentPosition = ("D", 5);
            // Remove pawn @ (B,6) -> (B,5)
            GamePiece? PlayerOnePawnTwo = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("B", 6)).FirstOrDefault();
            PlayerOnePawnTwo.CurrentPosition = ("B", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneBishop.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneBishop.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            // Assert
            PlayerOneBishop.AllowedMovement.Count().Should().Be(7);
        }
        [Fact]
        public void BishopShouldHaveEightValidMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();
            // Remove pawn @ (D,1) -> (D,2)
            GamePiece? PlayerTwoPawnOne = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawnOne.CurrentPosition = ("D", 2);
            // Remove pawn @ (B,1) -> (D,2)
            GamePiece? PlayerTwoPawnTwo = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("B", 1)).FirstOrDefault();
            PlayerTwoPawnTwo.CurrentPosition = ("B", 2);
            game.PlaceInMatrix();
            game.CurrentPlayer = PlayerTwo;
            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoBishop.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerTwoBishop.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerTwo);

            // Assert
            PlayerTwoBishop.AllowedMovement.Count().Should().Be(7);
        }
        // Unfinished
        [Fact]
        public void BishopShouldMovePlayerOnePieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneBishop.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneBishop.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);

            // Assert
            PlayerOneBishop.CurrentPosition.Should().Be(("A", 5));
            game.Board.Matrix[0, 5].Should().Be(PlayerOneBishop);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void BishopShouldMovePlayerTwoPieceToValidPosition()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            game.PlaceInMatrix();

            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneBishop.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneBishop.CurrentPosition.Y;

            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoBishop.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoBishop.CurrentPosition.Y;

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(0, 5, game);
            game.CurrentPlayer = PlayerTwo;
            PlayerTwo.Select((int)X2, Y2, game, PlayerTwo);
            PlayerTwo.MovePiece(0, 2, game);

            // Assert
            PlayerTwoBishop.CurrentPosition.Should().Be(("A", 2));
            game.Board.Matrix[0, 2].Should().Be(PlayerTwoBishop);
            game.Board.Matrix[(int)X, Y].Should().BeNull();
        }
        [Fact]
        public void BishopShouldAllowBishopToAttack()
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
            GamePiece? PlayerOneBishop = PlayerOne!.GamePieces!.Where(p => p.Name == "Bishop").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneBishop.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneBishop.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoBishop.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoBishop.CurrentPosition.Y;

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
                    if (game.Board.Matrix[i, j] == PlayerTwoBishop) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneBishop.CurrentPosition.Should().Be(("B", 3));
            game.Board.Matrix[1, 3].Should().Be(PlayerOneBishop);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class Queen_Tests
    {
        [Fact]
        public void QueenShouldHaveNotHaveAnyMovesForBothPlayers()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();
            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerOneQueen.AllowedMovement.Count().Should().Be(0);
            PlayerTwoQueen.AllowedMovement.Count().Should().Be(0);
        }
        [Fact]
        public void QueenShouldHaveSixMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("E", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("F", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerOne);

            // Assert
            PlayerOneQueen.AllowedMovement.Count().Should().Be(6);

        }
        [Fact]
        public void QueenShouldHaveSixMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();

            // Move pawn @ (E,1) -> (F,2)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("E", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("F", 2);
            game.PlaceInMatrix();
            game.CurrentPlayer = PlayerTwo;

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerTwoQueen.AllowedMovement.Count().Should().Be(6);

        }
        [Fact]
        public void QueenShouldHave20MovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 7)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("E", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("F", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerOne);
            game.CurrentPlayer.MovePiece(4, 4, game);
            game.CurrentPlayer = PlayerOne;
            game.CurrentPlayer.Select(4, 4, game, PlayerOne);
            // Assert
            PlayerOneQueen.AllowedMovement.Count().Should().Be(20);

        }
        [Fact]
        public void QueenShouldHave20MovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen" && p.CurrentPosition == ("E", 0)).FirstOrDefault();

            // Move pawn @ (E,1) -> (F,2)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("E", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("F", 2);
            game.PlaceInMatrix();
            game.CurrentPlayer = PlayerTwo;

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoQueen.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);
            game.CurrentPlayer.MovePiece(4, 3, game);
            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(4, 3, game, PlayerTwo);
            // Assert
            PlayerTwoQueen.AllowedMovement.Count().Should().Be(20);

        }


        // Unfinished
        [Fact]
        public void QueenShouldAllowQueenToAttack()
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
            GamePiece? PlayerOneQueen = PlayerOne!.GamePieces!.Where(p => p.Name == "Queen").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerOneQueen.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneQueen.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoQueen = PlayerTwo!.GamePieces!.Where(p => p.Name == "Queen").FirstOrDefault();
            Enum.TryParse<ChessCoordinate>(PlayerTwoQueen.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoQueen.CurrentPosition.Y;

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
                    if (game.Board.Matrix[i, j] == PlayerTwoQueen) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneQueen.CurrentPosition.Should().Be(("B", 3));
            game.Board.Matrix[1, 3].Should().Be(PlayerOneQueen);
            RemovedFromPlay.Should().BeNull();
        }
    }
    public class King_Tests
    {
        [Fact]
        public void KingShouldHaveNotHaveAnyMovesForBothPlayers()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);
            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(0);
            PlayerTwoKing.AllowedMovement.Count().Should().Be(0);
        }
        [Fact]
        public void KingShouldHaveOneMove()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerOnePawn = PlayerOne!.GamePieces!.Where(p => p.CurrentPosition == ("D", 6)).FirstOrDefault();
            PlayerOnePawn.CurrentPosition = ("D", 5);

            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();

            // Move pawn @ (E,6) -> (F,5)
            GamePiece? PlayerTwoPawn = PlayerTwo!.GamePieces!.Where(p => p.CurrentPosition == ("D", 1)).FirstOrDefault();
            PlayerTwoPawn.CurrentPosition = ("D", 2);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X1);
            int Y1 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X1, Y1, game, PlayerOne);

            game.CurrentPlayer = PlayerTwo;

            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKing.CurrentPosition.Y;
            game.CurrentPlayer.Select((int)X2, Y2, game, PlayerTwo);

            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(1);
            PlayerTwoKing.AllowedMovement.Count().Should().Be(1);

        }
        [Fact]
        public void KingShouldHaveEightMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();

            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 4);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select(3, 4, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(8);

        }
        [Fact]
        public void KingShouldHaveEightMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();

            // Place King @ (D,4) 
            PlayerTwoKing.CurrentPosition = ("D", 3);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKing.CurrentPosition.Y;
            game.CurrentPlayer.Select(3, 3, game, PlayerTwo);
            // Assert
            PlayerTwoKing.AllowedMovement.Count().Should().Be(8);
        }
        [Fact]
        public void KingShouldHaveFiveMovesForPlayerOne()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 3);


            GamePiece? PlayerTwoRook = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRook.CurrentPosition = ("A", 2);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;
            game.CurrentPlayer.Select(3, 3, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(5);

        }
        [Fact]
        public void KingShouldHaveFiveMovesForPlayerTwo()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerTwoKing = PlayerTwo!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 0)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerTwoKing.CurrentPosition = ("D", 3);


            GamePiece? PlayerOneRook = PlayerOne!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 7)).FirstOrDefault();
            PlayerOneRook.CurrentPosition = ("A", 2);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerTwoKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoKing.CurrentPosition.Y;

            game.CurrentPlayer = PlayerTwo;
            game.CurrentPlayer.Select(3, 3, game, PlayerTwo);
            // Assert
            PlayerTwoKing.AllowedMovement.Count().Should().Be(5);
        }
        [Fact]
        public void KingShouldHaveNotHaveMovesAndTriggerGameOver()
        {
            // Arrange
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            // Place King @ (D,4) 
            PlayerOneKing.CurrentPosition = ("D", 3);


            GamePiece? PlayerTwoRookOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRookOne.CurrentPosition = ("A", 2);
            GamePiece? PlayerTwoRookTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Rook" && p.CurrentPosition == ("A", 0)).FirstOrDefault();
            PlayerTwoRookOne.CurrentPosition = ("A", 4);
            GamePiece? PlayerTwoKnightOne = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("B", 0)).FirstOrDefault();
            PlayerTwoKnightOne.CurrentPosition = ("B", 5);
            GamePiece? PlayerTwoKnightTwo = PlayerTwo!.GamePieces!.Where(p => p.Name == "Knight" && p.CurrentPosition == ("G", 0)).FirstOrDefault();
            PlayerTwoKnightOne.CurrentPosition = ("F", 5);
            game.PlaceInMatrix();

            // Act            
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerOneKing.CurrentPosition.Y;

            game.CurrentPlayer.Select(3, 3, game, PlayerOne);
            // Assert
            PlayerOneKing.AllowedMovement.Count().Should().Be(5);
            game.GameOver.Should().BeTrue();
        }

        [Fact]
        public void KingShouldAllowKingToAttack()
        {
            // Arrange

            // Setup Game
            Chess game = new();
            Player PlayerOne = new("P1");
            Player PlayerTwo = new("P2");
            game.JoinGame(PlayerOne);
            game.JoinGame(PlayerTwo);

            // Setup PlayerOne
            GamePiece? PlayerOneKing = PlayerOne!.GamePieces!.Where(p => p.Name == "King" && p.CurrentPosition == ("D", 7)).FirstOrDefault();
            PlayerOneKing.CurrentPosition = ("D", 3);
            Enum.TryParse<ChessCoordinate>(PlayerOneKing.CurrentPosition.X, out ChessCoordinate X);
            int Y = PlayerOneKing.CurrentPosition.Y;

            // Setup PlayerTwo
            GamePiece? PlayerTwoBishop = PlayerTwo!.GamePieces!.Where(p => p.Name == "Bishop" && p.CurrentPosition == ("C", 0)).FirstOrDefault();
            PlayerTwoBishop.CurrentPosition = ("E", 3);
            Enum.TryParse<ChessCoordinate>(PlayerTwoBishop.CurrentPosition.X, out ChessCoordinate X2);
            int Y2 = PlayerTwoBishop.CurrentPosition.Y;
            game.PlaceInMatrix();

            // Act
            PlayerOne.Select((int)X, Y, game, PlayerOne);
            PlayerOne.MovePiece(4, 3, game);
            GamePiece? RemovedFromPlay = null;
            for (int i = 0; i < game.Board.Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < game.Board.Matrix.GetLength(0); j++)
                {
                    if (game.Board.Matrix[i, j] == PlayerTwoBishop) RemovedFromPlay = game.Board.Matrix[i, j];
                }
            }
            // Assert
            PlayerOneKing.CurrentPosition.Should().Be(("E", 3));
            game.Board.Matrix[1, 3].Should().Be(PlayerOneKing);
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