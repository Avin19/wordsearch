using System;
using System.Collections.Generic;

namespace WordSearch
{
    public class LevelGenerator
    {
        internal enum WordDir { HOR, VER, DIAG }

        private int _gridSize;
        private char[][] _genGrid;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private Random _randomGen;

        public LevelGenerator(int gridSize)
        {
            _gridSize = gridSize;

            _genGrid = new char[gridSize][];
            for (int i = 0; i < gridSize; i++)
                _genGrid[i] = new char[gridSize];

            _randomGen = new Random();
        }

        // Initialize an empty grid with placeholders
        private void CreateEmptyGrid()
        {
            for (int r = 0; r < _gridSize; r++)
            {
                for (int c = 0; c < _gridSize; c++)
                    _genGrid[r][c] = '-';
            }
        }

        // Main function to generate the puzzle
        public char[][] Generate(List<string> wordList)
        {
            CreateEmptyGrid();

            // Sort words by length descending (place longest words first)
            // var sortedWords = words.OrderByDescending(w => w.Length).ToList();

            bool placed;
            int attempts;
            const int MAX_ATTEMPTS = 100; // Prevent infinite loops

            int wordDir, startRow, startCol;

            for (int i = 0; i < _gridSize; i++)
            {
                placed = false;
                attempts = 0;

                while (!placed && attempts < MAX_ATTEMPTS)
                {
                    wordDir = _randomGen.Next(2);

                    startRow = _randomGen.Next(_gridSize);
                    startCol = _randomGen.Next(_gridSize);

                    if (CanPlaceWord(wordList[i], startRow, startCol, wordDir))
                    {
                        PlaceWord(wordList[i], startRow, startCol, wordDir);
                        placed = true;
                    }
                    attempts++;
                }

                if (!placed)
                {
                    Console.WriteLine($"Warning: Could not place the word \"{wordList[i]}\". Grid might be too small.");
                }
            }

            FillRandomLetters();
            return _genGrid;
        }

        // Check if a word can fit without going out of bounds or colliding badly
        private bool CanPlaceWord(string word, int row, int col, int direction)
        {
            int len = word.Length;

            // Check Out of Bounds
            int rowMult = 0, colMult = 0;
            // /*
            switch ((WordDir)direction)
            {
                case WordDir.HOR:
                    if ((col + len) > _gridSize) return false;
                    colMult = 1;
                    rowMult = 0;

                    break;

                case WordDir.VER:
                    if ((row + len) > _gridSize) return false;
                    colMult = 0;
                    rowMult = 1;

                    break;

                case WordDir.DIAG:
                    if ((row + len) > _gridSize) return false;

                    break;
            }
            // */

            // if (direction == (int)WordDir.HOR && (col + len) > _gridSize) return false;           // Horizontal
            // else if (direction == (int)WordDir.VER && (row + len) > _gridSize) return false;      // Vertical
            // else if (direction == (int)WordDir.DIAG && (row + len) > _gridSize) return false;     // Diagonal

            int r, c;

            // Check for collisions
            for (int i = 0; i < len; i++)
            {
                // r = direction == 1 ? row + i : row;
                // c = direction == 0 ? col + i : col;

                r = row + (i * rowMult);
                c = col + (i * colMult);

                char currentCell = _genGrid[r][c];

                // If the cell is not empty AND it's not the same letter, we have a collision
                if (currentCell != '-' && currentCell != word[i])
                    return false;
            }
            return true;
        }

        // Actually place the word in the grid
        private void PlaceWord(string word, int row, int col, int direction)
        {
            int rowMult = 0, colMult = 0;
            switch ((WordDir)direction)
            {
                case WordDir.HOR:
                    colMult = 1;
                    rowMult = 0;

                    break;

                case WordDir.VER:
                    colMult = 0;
                    rowMult = 1;

                    break;

                case WordDir.DIAG:
                    colMult = 1;
                    rowMult = 1;

                    break;
            }

            int r, c;
            for (int i = 0; i < word.Length; i++)
            {
                // int r = direction == 1 ? row + i : row;
                // int c = direction == 0 ? col + i : col;

                r = row + (i * rowMult);
                c = col + (i * colMult);

                _genGrid[r][c] = word[i];
            }
        }

        // Fill all remaining '-' cells with random alphabets
        private void FillRandomLetters()
        {
            for (int r = 0; r < _gridSize; r++)
            {
                for (int c = 0; c < _gridSize; c++)
                {
                    if (_genGrid[r][c] == '-')
                        _genGrid[r][c] = Alphabet[_randomGen.Next(Alphabet.Length)];
                }
            }
        }

        public void PrintGrid()
        {
            for (int r = 0; r < _gridSize; r++)
            {
                for (int c = 0; c < _gridSize; c++)
                    Console.Write(_genGrid[r][c] + " ");
                Console.WriteLine();
            }
        }
    }
}