namespace Chess_Final.Tests;
using FluentAssertions;
public class UnitTest1
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
}