namespace Chess_Final.Tests;
using Player;
using FluentAssertions;
using Chess_Final.Generics;
using Chess_Final.Chess;

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