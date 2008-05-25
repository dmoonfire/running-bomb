using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace RunningBomb.Themes
{
	/// <summary>
	/// Consolidated single system that takes into accounts
	/// variables and the context.
	///
	/// This string must be in the format of:
	///    [variable][(*|/)single][(+|-)single]
	/// </summary>
	public class ThemeSingle
	{
		#region Constants
		private static readonly Regex Format = new Regex(
			@"(([a-z]+)((\*|/)(\d+(\.\d+)?))?)?((\+|-)?(\d+(\.\d+)?))?");
		#endregion

		#region Constructors
		/// <summary>
		/// Creates the single and parses the results.
		/// </summary>
		public ThemeSingle(string single)
		{
			// Simplify the results
			single = single.Replace(" ", "");

			if (single.Length == 0)
				throw new Exception("Cannot assign an empty string");

			// Parse the results
			if (!Format.IsMatch(single))
				throw new Exception("Given input does not match format");

			// Get the results from the regex
			Match m = Format.Match(single);
			string varStr = m.Groups[2].Value;
			string op1Str = m.Groups[4].Value;
			string num1Str = m.Groups[5].Value;
			string op2Str = m.Groups[8].Value;
			string num2Str = m.Groups[9].Value;

			// Parse the results
			if (!String.IsNullOrEmpty(op1Str) &&
				!String.IsNullOrEmpty(num1Str))
			{
				op1 = op1Str;
				num1 = Single.Parse(num1Str);
			}

			if (!String.IsNullOrEmpty(op2Str))
				op2 = op2Str;

			if (!String.IsNullOrEmpty(num2Str))
				num2 = Single.Parse(num2Str);

			if (!String.IsNullOrEmpty(varStr))
				variable = varStr;
		}
		#endregion

		#region Properties
		private string variable = null;
		private string op1 = "*";
		private float num1 = 1;
		private string op2 = "+";
		private float num2 = 0;
		#endregion

		#region Values
		/// <summary>
		/// Retrieves the value for the variable from the context.
		/// </summary>
		public float GetValue(ThemeContext context)
		{
			// Start variable
			float value = num2;

			if (op2 == "-")
				value = -value;

			// Process the variables
			if (variable != null)
			{
				// Grab the variable
				float vval = Convert.ToSingle(context[variable]);

				// Apply the operation
				if (op1 == "*")
					vval *= num1;
				else
					vval /= num1;

				// Add it to the results (since we already negate for
				// negatives)
				value += vval;
			}

			// Return the results
			return value;
		}
		#endregion
	}
}
