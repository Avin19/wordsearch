using System;

namespace WordSearch
{
    [Serializable]
    public class LevelGenerator
    {
        internal enum WordDir { HOR, VER, DIAG }

        private int _gridSize;
        private char[][] _genGrid;
        private Random _randomGen;

        private const int ASCII_A = 65, ALPHABETS = 26;
        private const int VER_FLAG = 0, DIAG_FLAG = 1;
        private const int LENGTH_VAL_OFFSET = 10, ROW_VAL_OFFSET = 10, ORIENTATION_OFFSET = 2;
        // private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //              TEST
        private int _successPasses, _failedPasses, _totalAttempts;

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
        public void Generate(string[] wordList, out char[][] grid, out long[] solutionArr)
        {
            CreateEmptyGrid();

            // Sort words by length descending (place longest words first)
            // var sortedWords = words.OrderByDescending(w => w.Length).ToList();

            bool placed;
            int attempts;
            const int MAX_ATTEMPTS = 100; // Prevent infinite loops

            grid = null;
            solutionArr = new long[_gridSize];

            int wordDir, startRow = 0, startCol = 0;

            for (int i = 0; i < _gridSize; i++)
            {
                placed = false;
                attempts = 0;

                while (!placed && attempts < MAX_ATTEMPTS)
                {
                    wordDir = (int)MathF.Round((_randomGen.Next(0, 150) / 100f) + 0.35f);

                    startRow = _randomGen.Next(_gridSize);
                    startCol = _randomGen.Next(_gridSize);

                    placed = TryPlaceWord(wordList[i], startRow, startCol, wordDir, ref solutionArr[i]);

                    attempts++;
                    _totalAttempts++;
                }

                if (!placed)
                {
                    _failedPasses |= (1 << i);
                    i--;                            // Decrement to allow another word to fill
                    // Console.WriteLine($"Warning: Could not place the word \"{wordList[i]}\". Grid might be too small.");
                }
                else
                {
                    _successPasses |= (1 << i);

                    // First set the row value
                    solutionArr[i] |= (1L << (startRow + ROW_VAL_OFFSET + ORIENTATION_OFFSET));
                    // Set the col value
                    solutionArr[i] |= (1L << (startCol + ORIENTATION_OFFSET));
                    // Set the length value
                    solutionArr[i] |= (1L << (wordList[i].Length - 1 + LENGTH_VAL_OFFSET + ROW_VAL_OFFSET + ORIENTATION_OFFSET));
                }
            }

            FillRandomLetters();
            grid = _genGrid;
        }

        // Check if a word can fit without going out of bounds or colliding badly
        private bool TryPlaceWord(string word, int row, int col, int direction, ref long solution)
        {
            int len = word.Length;

            // Check Out of Bounds
            int rowMult = 0, colMult = 0;
            switch ((WordDir)direction)
            {
                case WordDir.HOR:
                    if ((col + len) > _gridSize) return false;
                    colMult = 1;
                    rowMult = 0;
                    // No flag, so this is HORIZONTAL

                    break;

                case WordDir.VER:
                    if ((row + len) > _gridSize) return false;
                    colMult = 0;
                    rowMult = 1;
                    solution |= (1 << VER_FLAG);

                    break;

                case WordDir.DIAG:
                    // Right along the middle
                    if ((row >= col) && (row + len) > _gridSize)
                        return false;
                    // Below the middle diagonal
                    else if ((col + len) > _gridSize)
                        return false;

                    colMult = 1;
                    rowMult = 1;
                    solution |= (1 << DIAG_FLAG);

                    break;
            }


            int r, c;
            // Check for collisions
            for (int i = 0; i < len; i++)
            {
                r = row + (i * rowMult);
                c = col + (i * colMult);

                char currentCell = _genGrid[r][c];

                // If the cell is not empty AND it's not the same letter, we have a collision
                if (currentCell != '-' && currentCell != word[i])
                    return false;
            }

            // Place the word
            for (int i = 0; i < word.Length; i++)
            {
                r = row + (i * rowMult);
                c = col + (i * colMult);

                _genGrid[r][c] = word[i];
            }
            return true;
        }

        // Fill all remaining '-' cells with random alphabets
        private void FillRandomLetters()
        {
            for (int r = 0; r < _gridSize; r++)
            {
                for (int c = 0; c < _gridSize; c++)
                {
                    if (_genGrid[r][c] == '-')
                        _genGrid[r][c] = (char)(ASCII_A + _randomGen.Next(ALPHABETS));
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