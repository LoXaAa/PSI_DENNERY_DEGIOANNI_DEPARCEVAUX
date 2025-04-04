using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PSI_RENDU1;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            [Fact]
            public void Dijkstra_CalculsDistancesCorrectes()
            {
                // Arrange
                var graphe = new Graphe<string>();
                graphe.AjouterNoeud("A");
                graphe.AjouterNoeud("B");
                graphe.AjouterNoeud("C");
                graphe.AjouterNoeud("D");

                graphe.AjouterLien("A", "B", 1);
                graphe.AjouterLien("A", "C", 4);
                graphe.AjouterLien("B", "C", 2);
                graphe.AjouterLien("B", "D", 5);
                graphe.AjouterLien("C", "D", 1);

                // Act
                var (distances, precedent) = graphe.Dijkstra("A");

                // Assert
                Assert.Equal(0, distances["A"]);
                Assert.Equal(1, distances["B"]);
                Assert.Equal(3, distances["C"]);
                Assert.Equal(4, distances["D"]);

                Assert.Null(precedent["A"]);
                Assert.Equal("A", precedent["B"]);
                Assert.Equal("B", precedent["C"]);
                Assert.Equal("C", precedent["D"]);
            }
        }
    }
}
