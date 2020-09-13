using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Framework.Writer
{
	public class ScriptWriter
	{
		private string target = string.Empty;
		private Dictionary<string, object> variableLookup = new Dictionary<string, object>();
		
		private enum LoopType
		{
			None = 0x0,
			ForEach = 0x1,
			ForEvery = 0x2
		}
		
		private LoopType _loopType = LoopType.None;
		private object[] _iteratee;
		private int _currentIterateeIndex;
		private int _loopStart = -1;
		private bool _isEmpty;

		public ScriptWriter(string text)
		{
			target = text;
		}

		public void AddVariable(string key, object val)
		{
			variableLookup.Add(key, val);
		}

		public string Parse()
		{
			var lines = new List<string>(target.Replace("\r\n", "\n").Split('\n'));
			var finalLines = new List<string>(lines.Count);

			var offset = 0;
			var parsed = false;
			var skipLine = false;
			var foundState = LoopType.None;
			
			StringBuilder sb;
			for (int i = 0; i < lines.Count; i++)
			{
				parsed = false;
				skipLine = false;
				sb = new StringBuilder(lines[i]);

				while (true)
				{
					string current = sb.ToString();
					int parseStart = current.IndexOf(">:", offset);
					
					if (parseStart < 0) break;

					parseStart += 2;
					int parseEnd = current.IndexOf(":<", offset + parseStart);

					if (parseEnd < 0)
					{
						throw new Exception("There was a parse start but no end on line " + (i + 1));
					}

					string contents = current.Substring(parseStart, parseEnd - parseStart);

					sb.Remove(parseStart - 2, parseEnd - parseStart + 4);

					if (CheckState(contents, ref foundState))
					{
						skipLine = true;

						// If we have left the loop
						if (foundState == LoopType.None)
						{
							if (_loopStart == -1) continue;

							if (++_currentIterateeIndex >= _iteratee.Length)
							{
								_loopType &= ~(LoopType.ForEach | LoopType.ForEvery);
								_iteratee = null;
								_loopStart = -1;
								_isEmpty = false;
							}
							else
							{
								i = _loopStart - 1;
								break;
							}
						}
						else if (foundState == LoopType.ForEach || foundState == LoopType.ForEvery)
						{
							_loopStart = i + 1;
						}

						continue;
					}

					if (!_isEmpty)
					{
						sb.Insert(parseStart - 2, ParseLine(contents));
					}

					parsed = true;
				}

				string built = sb.ToString();

				if (parsed && built.Trim().Length == 0)
				{
					lines.RemoveAt(i--);
				}
				else if (!skipLine && !_isEmpty)
				{
					finalLines.Add(built);
				}
			}

			return string.Join(Environment.NewLine, finalLines.ToArray());
		}

		private bool CheckState(string contents, ref LoopType foundLoopType)
		{
			if (contents.StartsWith("ENDFOREACH"))
			{
				if ((_loopType & LoopType.ForEach) == 0)
				{
					throw new Exception("A foreach has ended before the start of the loop");
				}

				foundLoopType = LoopType.None;
				return true;
			}
			
			if (contents.StartsWith("ENDFOREVERY"))
			{
				if ((_loopType & LoopType.ForEvery) == 0)
				{
					throw new Exception("A foreach has ended before the start of the loop");
				}

				foundLoopType = LoopType.None;
				return true;
			}
			
			if (contents.StartsWith("FOREACH"))
			{
				if ((_loopType & LoopType.ForEach) != 0 || (_loopType & LoopType.ForEvery) != 0)
				{
					throw new Exception("A loop is already in execution and in this version nested foreach loops are not allowed");
				}

				_loopType |= LoopType.ForEach;

				var iterateeName = contents.TrimStart("FOREACH ".ToCharArray());

				if (!variableLookup.ContainsKey(iterateeName))
				{
					throw new Exception("No variable with the name " + iterateeName + " could be located");
				}

				_iteratee = (object[])variableLookup[iterateeName];

				if (_iteratee.Length == 0)
				{
					_isEmpty = true;
				}

				_currentIterateeIndex = 0;
				foundLoopType = LoopType.ForEach;
				return true;
			}
			
			if (contents.StartsWith("FOREVERY"))
			{
				if ((_loopType & LoopType.ForEach) != 0 || (_loopType & LoopType.ForEvery) != 0)
					throw new Exception("A loop is already in execution and in this version nested foreach loops are not allowed");

				_loopType |= LoopType.ForEvery;

				var iterateeName = contents.TrimStart("FOREVERY ".ToCharArray());

				if (!variableLookup.ContainsKey(iterateeName))
					throw new Exception("No variable with the name " + iterateeName + " could be located");

				_iteratee = (object[])variableLookup[iterateeName];

				if (_iteratee.Length == 0 || ((object[])_iteratee[0]).Length == 0)
					_isEmpty = true;

				_currentIterateeIndex = 0;
				foundLoopType = LoopType.ForEvery;
				return true;
			}

			return false;
		}

		private string ParseLine(string contents)
		{
			if (contents.StartsWith("[") && contents.EndsWith("]"))
			{
				if (contents == "[i]" && _iteratee != null)
				{
					return FormatReturn(_iteratee[_currentIterateeIndex]);
				}

				if (contents == "[idx]" && _iteratee != null)
				{
					return FormatReturn(_currentIterateeIndex);
				}

				var idxStr = contents.TrimStart('[').TrimEnd(']');
				if (int.TryParse(idxStr, out int idx))
				{
					return FormatReturn(((object[]) _iteratee[_currentIterateeIndex])[idx]);
				} 
				
				throw new Exception("The index " + idxStr + " is not an integer");
			}
			
			if (contents == "ELSEIF")
			{
				if (_currentIterateeIndex == 0)
				{
					return "if";
				} 
				
				return "else if";
			}

			if (variableLookup.ContainsKey(contents))
			{
				return FormatReturn(variableLookup[contents]);
			}

			return string.Empty;
		}

		private string FormatReturn(object data)
		{
			if (data is bool)
			{
				return data.ToString().ToLower();
			}
			
			if (data is float)
			{
				float fData = (float) data;
				return fData.ToString(CultureInfo.InvariantCulture) + "f";
			}
			
			return data.ToString();
		}
	}
}