using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace New_York_Times
{
	/// <summary>
	/// Console-based NYT-style Crossword game.
	/// Loads a puzzle file, renders the grid and clues, and provides commands to fill, check, and reveal answers.
	/// </summary>
	internal class Crossword : Game
	{
		#region Variables
		/// <summary>
		/// The solution grid containing letters and '#' for blocks.
		/// </summary>
		private char[,] Solution = new char[0, 0];

		/// <summary>
		/// The player's current grid entries. Empty cells contain '\0'.
		/// </summary>
		private char[,] Current = new char[0, 0];

		/// <summary>
		/// Across entries keyed by clue number.
		/// </summary>
		private readonly Dictionary<int, CrosswordEntry> Across = [];

		/// <summary>
		/// Down entries keyed by clue number.
		/// </summary>
		private readonly Dictionary<int, CrosswordEntry> Down = [];

		/// <summary>
		/// Number of rows in the grid.
		/// </summary>
		private int Rows => Solution.GetLength(0);

		/// <summary>
		/// Number of columns in the grid.
		/// </summary>
		private int Cols => Solution.GetLength(1);
		#endregion

		/// <summary>
		/// Initializes the crossword and attempts to load the puzzle from the default game state file.
		/// </summary>
		public Crossword() : base()
		{
			// Try to load puzzle from file; if not found, we just show instructions in Main
			if (File.Exists(filePath))
			{
				TryLoadPuzzle(filePath, out string error);
				if (error is { Length: > 0 })
				{
					Console.Error.WriteLine($"Failed to load crossword: {error}");
				}
			}
		}

		/// <summary>
		/// Entry point for the game loop. Renders the board/clues and processes user commands.
		/// </summary>
		public override void Main()
		{
			if (Rows == 0 || Cols == 0)
			{
				ShowFileFormatHelp();
				return;
			}

			while (true)
			{
				Console.Clear();
				RenderBoard();
				RenderClues();
				Console.WriteLine();
				Console.WriteLine("Commands:");
				Console.WriteLine("  A <num> <answer>   -> fill an Across entry");
				Console.WriteLine("  D <num> <answer>   -> fill a Down entry");
				Console.WriteLine("  C                  -> check all filled letters");
				Console.WriteLine("  R <A|D> <num>      -> reveal a single entry");
				Console.WriteLine("  CL                 -> clear the whole grid");
				Console.WriteLine("  ESC                -> exit");
				Console.WriteLine();
				Console.Write("Enter command: ");
				string input = Console.ReadLine() ?? string.Empty;

				if (string.IsNullOrEmpty(input))
				{
					continue;
				}

				string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				string command = parts[0].ToUpperInvariant();

				if (command == "ESC")
				{
					return;
				}

				switch (command)
				{
					case "A":
					case "D":
						if (parts.Length < 3)
						{
							PauseWithMessage("Format: A|D <number> <answer>");
							break;
						}
						int num;
						if (!int.TryParse(parts[1], out num))
						{
							PauseWithMessage("Format: A|D <number> <answer>");
							break;
						}
						// Join remaining tokens without spaces; punctuation is filtered in TryFillEntry
						string answer = string.Join("", parts, 2, parts.Length - 2);
						bool isAcross = command == "A";
						if (!TryFillEntry(isAcross, num, answer, out string err))
						{
							PauseWithMessage(err ?? "Could not fill entry.");
						}
						else if (IsComplete())
						{
							Console.Clear();
							RenderBoard(true);
							Console.WriteLine();
							Console.WriteLine("Puzzle complete! Great job.");
							PauseWithMessage("Press any key to return to menu...");
							return;
						}
						break;

					case "C":
						CheckAll();
						break;

					case "R":
						if (parts.Length < 3)
						{
							PauseWithMessage("Format: R <A|D> <number>");
							break;
						}
						string ad = parts[1].ToUpperInvariant();
						int n;
						if ((ad != "A" && ad != "D") || !int.TryParse(parts[2], out n))
						{
							PauseWithMessage("Format: R <A|D> <number>");
							break;
						}
						Reveal(ad == "A", n);
						break;

					case "CL":
						Array.Clear(Current, 0, Current.Length);
						break;

					default:
						PauseWithMessage("Unknown command.");
						break;
				}
			}
		}

		/// <summary>
		/// Determines whether a cell in the solution grid is a block.
		/// </summary>
		/// <param name="r">Row index.</param>
		/// <param name="c">Column index.</param>
		/// <returns>True if the cell contains '#'; otherwise false.</returns>
		private bool IsBlock(int r, int c)
		{
			return Solution[r, c] == '#';
		}

		/// <summary>
		/// Builds a horizontal separator line for the ASCII grid (e.g., +---+---+).
		/// </summary>
		/// <returns>Separator line string.</returns>
		private string BuildHorizontalLine()
		{
			StringBuilder sb = new StringBuilder(Cols * 4 + 1);
			for (int c = 0; c < Cols; c++)
			{
				sb.Append("+---");
			}
			sb.Append('+');
			return sb.ToString();
		}

		/// <summary>
		/// Renders the crossword grid to the console using an ASCII table with no colors.
		/// Blocks are shown as '#'.

		/// </summary>
		/// <param name="showAll">If true, show the full solution; otherwise show current entries.</param>
		private void RenderBoard(bool showAll = false)
		{
			Console.WriteLine($"Crossword {Rows}x{Cols}");
			Console.WriteLine();

			string sep = BuildHorizontalLine();

			for (int r = 0; r < Rows; r++)
			{
				// Row separator
				Console.WriteLine(sep);

				// Row content
				StringBuilder rowBuilder = new StringBuilder(Cols * 4 + 1);
				rowBuilder.Append('|');
				for (int c = 0; c < Cols; c++)
				{
					char toShow;
					if (IsBlock(r, c))
					{
						toShow = '#';
					}
					else
					{
						char sol = char.ToUpperInvariant(Solution[r, c]);
						char cur = char.ToUpperInvariant(Current[r, c]);
						toShow = showAll ? sol : (cur == '\0' ? ' ' : cur);
					}

					rowBuilder.Append(' ');
					rowBuilder.Append(toShow);
					rowBuilder.Append(' ');
					rowBuilder.Append('|');
				}
				Console.WriteLine(rowBuilder.ToString());
			}
			// Closing separator
			Console.WriteLine(sep);
		}

		/// <summary>
		/// Renders the Across and Down clues with their lengths.
		/// </summary>
		private void RenderClues()
		{
			Console.WriteLine();
			Console.WriteLine("ACROSS:");
			List<int> acrossKeys = [.. Across.Keys];
			acrossKeys.Sort();
			for (int i = 0; i < acrossKeys.Count; i++)
			{
				int key = acrossKeys[i];
				CrosswordEntry entry = Across[key];
				Console.WriteLine($"{key}. {entry.Clue} ({entry.Length})");
			}

			Console.WriteLine();
			Console.WriteLine("DOWN:");
			List<int> downKeys = [.. Down.Keys];
			downKeys.Sort();
			for (int i = 0; i < downKeys.Count; i++)
			{
				int key = downKeys[i];
				CrosswordEntry entry = Down[key];
				Console.WriteLine($"{key}. {entry.Clue} ({entry.Length})");
			}
		}

		/// <summary>
		/// Writes a message and waits for a key press.
		/// </summary>
		/// <param name="message">The message to display.</param>
		private static void PauseWithMessage(string message)
		{
			Console.WriteLine(message);
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(true);
		}

		/// <summary>
		/// Attempts to fill an Across/Down entry from an answer string.
		/// </summary>
		/// <param name="isAcross">True to fill an Across entry; false for Down.</param>
		/// <param name="number">The clue number.</param>
		/// <param name="answer">The user's raw answer (letters and non-letters allowed).</param>
		/// <param name="error">Output error message if filling fails.</param>
		/// <returns>True if filled successfully; otherwise false.</returns>
		private bool TryFillEntry(bool isAcross, int number, string answer, out string? error)
		{
			error = null;
			Dictionary<int, CrosswordEntry> map = isAcross ? Across : Down;

			if (!map.TryGetValue(number, out CrosswordEntry entry))
			{
				error = $"Clue {number} {(isAcross ? "Across" : "Down")} not found.";
				return false;
			}

			// Normalize: keep letters only, uppercase
			List<char> chars = new(answer.Length);
			for (int i = 0; i < answer.Length; i++)
			{
				char ch = answer[i];
				if (char.IsLetter(ch))
				{
					chars.Add(char.ToUpperInvariant(ch));
				}
			}
			string normalized = new([.. chars]);

			if (normalized.Length != entry.Length)
			{
				error = $"Answer length must be {entry.Length}.";
				return false;
			}

			for (int i = 0; i < entry.Length; i++)
			{
				int rr = entry.Row + (entry.IsAcross ? 0 : i);
				int cc = entry.Col + (entry.IsAcross ? i : 0);
				Current[rr, cc] = normalized[i];
			}
			return true;
		}

		/// <summary>
		/// Checks the current grid and reports the number of filled and correct letters.
		/// </summary>
		private void CheckAll()
		{
			int filled = 0;
			int correct = 0;
			int total = 0;

			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Cols; c++)
				{
					if (IsBlock(r, c)) continue;
					total++;
					char sol = char.ToUpperInvariant(Solution[r, c]);
					char cur = char.ToUpperInvariant(Current[r, c]);
					if (cur != '\0') filled++;
					if (cur != '\0' && cur == sol) correct++;
				}
			}
			PauseWithMessage($"Filled: {filled}/{total} | Correct so far: {correct}");
		}

		/// <summary>
		/// Reveals the solution letters for a specific Across/Down entry.
		/// </summary>
		/// <param name="isAcross">True for Across; false for Down.</param>
		/// <param name="number">The clue number to reveal.</param>
		private void Reveal(bool isAcross, int number)
		{
			Dictionary<int, CrosswordEntry> map = isAcross ? Across : Down;

			CrosswordEntry entry;
			if (!map.TryGetValue(number, out entry))
			{
				PauseWithMessage("No such clue number.");
				return;
			}

			for (int i = 0; i < entry.Length; i++)
			{
				int rr = entry.Row + (entry.IsAcross ? 0 : i);
				int cc = entry.Col + (entry.IsAcross ? i : 0);
				Current[rr, cc] = char.ToUpperInvariant(Solution[rr, cc]);
			}
		}

		/// <summary>
		/// Determines whether the player's current grid matches the solution.
		/// </summary>
		/// <returns>True if every non-block cell matches; otherwise false.</returns>
		private bool IsComplete()
		{
			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Cols; c++)
				{
					if (IsBlock(r, c)) continue;
					if (char.ToUpperInvariant(Current[r, c]) != char.ToUpperInvariant(Solution[r, c]))
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Prints the expected puzzle file format and a sample to help authors create a puzzle.
		/// </summary>
		private void ShowFileFormatHelp()
		{
			Console.WriteLine("No crossword file found.");
			Console.WriteLine($"Create: {filePath}");
			Console.WriteLine();
			Console.WriteLine("Expected format:");
			Console.WriteLine("  First non-empty line: <rows>x<cols> e.g. 5x5");
			Console.WriteLine("  Next <rows> lines: the grid using letters for solution and # for blocks");
			Console.WriteLine("  Then a line 'ACROSS:' followed by lines '<number>. <clue>'");
			Console.WriteLine("  Then a line 'DOWN:' followed by lines '<number>. <clue>'");
			Console.WriteLine();
			Console.WriteLine("Example:");
			Console.WriteLine("5x5");
			Console.WriteLine("HELLO");
			Console.WriteLine("E#A#R");
			Console.WriteLine("LION#");
			Console.WriteLine("L#I#E");
			Console.WriteLine("OCEAN");
			Console.WriteLine("ACROSS:");
			Console.WriteLine("1. Greeting");
			Console.WriteLine("3. Vowel after D");
			Console.WriteLine("5. King of the jungle");
			Console.WriteLine("7. H2O, poetically");
			Console.WriteLine("DOWN:");
			Console.WriteLine("1. Cry of surprise");
			Console.WriteLine("2. Network of routes");
			Console.WriteLine("4. Exist");
			Console.WriteLine("6. Negative vote");
			Console.WriteLine();
			Console.WriteLine("Numbers must match the automatic numbering (left-to-right, top-to-bottom) for starting cells.");
		}

		/// <summary>
		/// Builds the Across and Down entries by scanning the grid and assigning standard numbering.
		/// </summary>
		/// <param name="acrossClues">Map of Across clue numbers to clue text.</param>
		/// <param name="downClues">Map of Down clue numbers to clue text.</param>
		private void BuildEntries(Dictionary<int, string> acrossClues, Dictionary<int, string> downClues)
		{
			Across.Clear();
			Down.Clear();

			Current = new char[Rows, Cols];

			int nextNumber = 1;
			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Cols; c++)
				{
					if (IsBlock(r, c)) continue;

					bool startsAcross = c == 0 || IsBlock(r, c - 1);
					bool startsDown = r == 0 || IsBlock(r - 1, c);
					if (!startsAcross && !startsDown) continue;

					int number = nextNumber++;

					if (startsAcross)
					{
						int len = 0;
						while (c + len < Cols && !IsBlock(r, c + len)) len++;
						CrosswordEntry entryA = new()
						{
							Number = number,
							Row = r,
							Col = c,
							IsAcross = true,
							Length = len,
							Clue = acrossClues.TryGetValue(number, out string value) ? value : $"Across {number}"
						};
						Across[number] = entryA;
					}

					if (startsDown)
					{
						int len = 0;
						while (r + len < Rows && !IsBlock(r + len, c)) len++;
						CrosswordEntry entryD = new()
						{
							Number = number,
							Row = r,
							Col = c,
							IsAcross = false,
							Length = len,
							Clue = downClues.TryGetValue(number, out string value) ? value : $"Down {number}"
						};
						Down[number] = entryD;
					}
				}
			}
		}

		/// <summary>
		/// Loads a puzzle from the given path and initializes the grid and clues.
		/// </summary>
		/// <param name="path">Path to the puzzle file.</param>
		/// <param name="error">Output error message if loading fails.</param>
		/// <returns>True if loaded successfully; otherwise false.</returns>
		private bool TryLoadPuzzle(string path, out string? error)
		{
			error = null;
			try
			{
				string[] lines = File.ReadAllLines(path);

				int idx = 0;
				// Find first non-empty as size line
				while (idx < lines.Length && string.IsNullOrWhiteSpace(lines[idx])) idx++;
				if (idx >= lines.Length)
				{
					error = "Empty file.";
					return false;
				}

				string sizeLine = lines[idx].Trim();
				idx++;
				if (!TryParseSize(sizeLine, out int rows, out int cols))
				{
					error = $"Invalid size line '{sizeLine}'. Expected e.g. '5x5'.";
					return false;
				}

				if (idx + rows > lines.Length)
				{
					error = "Not enough grid rows in file.";
					return false;
				}

				Solution = new char[rows, cols];
				for (int r = 0; r < rows; r++, idx++)
				{
					string row = (lines[idx] ?? string.Empty).Trim();
					if (row.Length != cols)
					{
						error = $"Row {r + 1} has length {row.Length}, expected {cols}.";
						return false;
					}
					for (int c = 0; c < cols; c++)
					{
						char ch = row[c];
						// Allow '.' but treat as space (should generally be letters or '#')
						Solution[r, c] = ch == '.' ? ' ' : ch;
					}
				}

				// Read ACROSS:
				while (idx < lines.Length && !string.Equals(lines[idx].Trim(), "ACROSS:", StringComparison.OrdinalIgnoreCase)) idx++;
				Dictionary<int, string> acrossClues = new Dictionary<int, string>();
				Dictionary<int, string> downClues = new Dictionary<int, string>();

				if (idx < lines.Length && string.Equals(lines[idx].Trim(), "ACROSS:", StringComparison.OrdinalIgnoreCase))
				{
					idx++;
					while (idx < lines.Length && !string.Equals(lines[idx].Trim(), "DOWN:", StringComparison.OrdinalIgnoreCase))
					{
						ParseClueLine(lines[idx], acrossClues);
						idx++;
					}
				}

				if (idx < lines.Length && string.Equals(lines[idx].Trim(), "DOWN:", StringComparison.OrdinalIgnoreCase))
				{
					idx++;
					while (idx < lines.Length)
					{
						ParseClueLine(lines[idx], downClues);
						idx++;
					}
				}

				BuildEntries(acrossClues, downClues);
				return true;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Solution = new char[0, 0];
				Current = new char[0, 0];
				Across.Clear();
				Down.Clear();
				return false;
			}
		}

		/// <summary>
		/// Parses a size token in the format "rowsxcols" or "rows cols".
		/// </summary>
		/// <param name="text">The size text.</param>
		/// <param name="rows">Output rows.</param>
		/// <param name="cols">Output columns.</param>
		/// <returns>True if parsed successfully; otherwise false.</returns>
		private static bool TryParseSize(string text, out int rows, out int cols)
		{
			rows = 0;
			cols = 0;
			text = text.Trim();
			int xIndexLower = text.IndexOf('x');
			int xIndexUpper = text.IndexOf('X');
			int sepIndex = xIndexLower >= 0 ? xIndexLower : xIndexUpper;
			if (sepIndex < 0)
			{
				string[] partsSpace = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				if (partsSpace.Length == 2 && int.TryParse(partsSpace[0], out rows) && int.TryParse(partsSpace[1], out cols) && rows > 0 && cols > 0)
				{
					return true;
				}
				return false;
			}

			string left = text.Substring(0, sepIndex).Trim();
			string right = text[(sepIndex + 1)..].Trim();
			return int.TryParse(left, out rows) && int.TryParse(right, out cols) && rows > 0 && cols > 0;
		}

		/// <summary>
		/// Parses a single clue line in the format "number. clue" and adds it to the dictionary.
		/// </summary>
		/// <param name="line">The clue line text.</param>
		/// <param name="dict">Dictionary to populate with parsed number and clue.</param>
		private static void ParseClueLine(string line, Dictionary<int, string> dict)
		{
			line = (line ?? string.Empty).Trim();
			if (string.IsNullOrWhiteSpace(line)) return;

			int dot = line.IndexOf('.');
			if (dot <= 0) return;

			string numberText = line.Substring(0, dot);
			if (!int.TryParse(numberText, out int n)) return;

			string clue = line[(dot + 1)..].Trim();
			if (clue.Length == 0) return;

			dict[n] = clue;
		}
	}

	/// <summary>
	/// Represents a single crossword entry (Across or Down).
	/// </summary>
	public class CrosswordEntry
	{
		/// <summary>
		/// The clue number assigned to this entry.
		/// </summary>
		public int Number { get; set; }

		/// <summary>
		/// Optional target word (not required since the solution is in the grid).
		/// </summary>
		public string Word { get; set; } = string.Empty;

		/// <summary>
		/// The clue text for this entry.
		/// </summary>
		public string Clue { get; set; } = string.Empty;

		/// <summary>
		/// Zero-based starting row of the entry.
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		/// Zero-based starting column of the entry.
		/// </summary>
		public int Col { get; set; }

		/// <summary>
		/// True if the entry runs across; false if it runs down.
		/// </summary>
		public bool IsAcross { get; set; }

		/// <summary>
		/// The number of letters in the entry.
		/// </summary>
		public int Length { get; set; }
	}
}
