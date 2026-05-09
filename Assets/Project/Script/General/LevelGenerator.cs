using System;
using System.Collections.Generic;
using System.Linq;

namespace WordSearch
{
    public class LevelGenerator
    {
        private int rows;
        private int cols;
        private char[,] grid;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private Random random;

        // Constructor to initialize the grid dimensions
        public LevelGenerator(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            this.grid = new char[rows, cols];
            this.random = new Random();
        }

        // Initialize an empty grid with placeholders
        private void CreateEmptyGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    grid[r, c] = '-';
                }
            }
        }

        // Main function to generate the puzzle
        public char[,] Generate(List<string> words)
        {
            CreateEmptyGrid();

            // Sort words by length descending (place longest words first)
            var sortedWords = words.OrderByDescending(w => w.Length).ToList();

            foreach (var originalWord in sortedWords)
            {
                string word = originalWord.ToUpper();
                bool placed = false;
                int attempts = 0;
                int maxAttempts = 100; // Prevent infinite loops

                while (!placed && attempts < maxAttempts)
                {
                    // 0 = Horizontal (Right), 1 = Vertical (Down)
                    int direction = random.Next(2);

                    int startRow = random.Next(rows);
                    int startCol = random.Next(cols);

                    if (CanPlaceWord(word, startRow, startCol, direction))
                    {
                        PlaceWord(word, startRow, startCol, direction);
                        placed = true;
                    }
                    attempts++;
                }

                if (!placed)
                {
                    Console.WriteLine($"Warning: Could not place the word \"{word}\". Grid might be too small.");
                }
            }

            FillRandomLetters();
            return grid;
        }

        // Check if a word can fit without going out of bounds or colliding badly
        private bool CanPlaceWord(string word, int row, int col, int direction)
        {
            int len = word.Length;

            // Check Out of Bounds
            if (direction == 0 && col + len > cols) return false; // Horizontal
            if (direction == 1 && row + len > rows) return false; // Vertical

            // Check for collisions
            for (int i = 0; i < len; i++)
            {
                int r = direction == 1 ? row + i : row;
                int c = direction == 0 ? col + i : col;

                char currentCell = grid[r, c];

                // If the cell is not empty AND it's not the same letter, we have a collision
                if (currentCell != '-' && currentCell != word[i])
                {
                    return false;
                }
            }
            return true;
        }

        // Actually place the word in the grid
        private void PlaceWord(string word, int row, int col, int direction)
        {
            for (int i = 0; i < word.Length; i++)
            {
                int r = direction == 1 ? row + i : row;
                int c = direction == 0 ? col + i : col;
                grid[r, c] = word[i];
            }
        }

        // Fill all remaining '-' cells with random alphabets
        private void FillRandomLetters()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r, c] == '-')
                    {
                        grid[r, c] = Alphabet[random.Next(Alphabet.Length)];
                    }
                }
            }
        }

        // Utility to print the grid nicely to the console
        public void PrintGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Console.Write(grid[r, c] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}