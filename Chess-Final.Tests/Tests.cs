namespace Chess_Final.Tests;
using Player;
using FluentAssertions;
using Chess_Final.Generics;
using Chess_Final.Chess;
using TC_DataManagerException;
using TC_DataManager;

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
        player.Name.Should().Be(newPlayerName);
    }

    [Fact]
    public void ShouldNotHaveAnyGamePieces()
    {
        // Arrange
        Player player = new Player("P1");
        // Act
        var result = player.GamePieces;
        // Assert
        result.Should().BeNull();

    }
}

public class GamePiece_Tests
{
    [Fact]
    public void ShouldHaveValidMove()
    {
        // Arrange

        // Act

        // Assert

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
    [Fact]
    public void ChessShouldInheritFromTypeGame()
    {
        // Arrange
        // Act
        Chess chess = new Chess();

        // Assert
        chess.Should().BeAssignableTo<Game>();
    }

    [Fact]
    public void CreatingNewGameShouldHaveInstanceOfCurrentGameNotEqualNull()
    {
        // Arrange
        // Act
        Chess chess = new Chess();

        // Assert
        Chess.CurrentGame.Should().NotBeNull();
    }
    [Fact]
    public void CreatingNewGameShouldHaveNewStaticInstanceOfSelf()
    {
        // Arrange
        // Act
        Chess chess = new Chess();

        // Assert
        Chess.CurrentGame.Equals(chess);
    }

    [Fact]
    public void PlayerOneShouldBeAbleToJoinGame()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        Player playerOne = new Player("John");
        chess.JoinGame(playerOne);
        // Assert
        chess.PlayerOne.Name.Should().Be("John");
    }
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
        chess.PlayerOne.Name.Should().Be("John");
        chess.PlayerTwo.Name.Should().Be("Jane");
    }

    [Fact]
    public void ChessBoardShouldCreateCorrectLayout()
    {
        // Arrange
        Chess chess = new Chess();
        // Act
        chess.LayoutGamePieces();
        // Assert
        chess.Board.Matrix![((int)ChessCoordinate.B), 0].Should().BeAssignableTo<ChessPieces.Pawn>();
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