using System;
using System.Collections.Generic;
using Assets.Sun.Base;
using UnityEngine;

namespace Assets.Sun.Task01
{
	public struct RangeWindow : IEquatable<RangeWindow>
	{
		private int _sourceSize;

		public int IndexBegin { get; private set; }
		public int IndexEnd { get; private set; }
		public int Step { get; private set; }
		public int Size =>
			Step != 0
				? 1 + (IndexEnd - IndexBegin) / Step
				: throw new NotSupportedException("step is zero");

		public bool IsIndexBeginInside => IndexBegin.IsIndexValid(_sourceSize);
		public bool IsIndexEndInside => IndexEnd.IsIndexValid(_sourceSize);

		public bool IsIndexBeginBound => !(IndexBegin - Step).IsIndexValid(_sourceSize);
		public bool IsIndexEndBound => !(IndexEnd + Step).IsIndexValid(_sourceSize);

		public static RangeWindow CreateFrom<T>(List<T> source)
		{
			return new RangeWindow { _sourceSize = source.Count, };
		}

		public bool IsValidCollectionIndex(int index)
		{
			return index.IsIndexValid(_sourceSize);
		}

		public bool Shift(int shift = 1)
		{
			var shiftA = shift * Step;
			IndexBegin += shiftA;
			IndexEnd += shiftA;
			return IsIndexBeginInside && IsIndexEndInside;
		}

		public void ExtendBegin(int extend = 1)
		{
			var index = IndexBegin + extend * Step;
			var clamped = Mathf.Clamp(index, int.MinValue, IndexEnd);
			if(index != clamped)
			{
				$"clamp index begin from {index} to {clamped}".Log();
			}
			IndexBegin = clamped;
		}

		public void ExtendEnd(int extend = 1)
		{
			var index = IndexEnd + extend * Step;
			var clamped = Mathf.Clamp(index, IndexBegin, int.MaxValue);
			if(index != clamped)
			{
				$"clamp index end from {index} to {clamped}".Log();
			}
			IndexEnd = index;
		}

		public void Rebuild(int indexBegin, int indexEnd, int step)
		{
			if(step == 0)
			{
				throw new NotSupportedException("step is zero");
			}

			Step = step;
			IndexBegin = indexBegin;
			IndexEnd = Mathf.Clamp(indexEnd, indexBegin, int.MaxValue);
			var size = Size;
			IndexEnd = (size - 1) * step + IndexBegin;
		}

		public override string ToString()
		{
			return $"index: [ {IndexBegin}, {IndexEnd} ] size: {Size} ( step: {Step} )";
		}

		public IEnumerator<int> GetEnumeratorRangeAll()
		{
			var current = IndexBegin;
			while(true)
			{
				yield return current;
				current += Step;
				if(current > IndexEnd)
				{
					break;
				}
			}
		}

		public IEnumerator<int> GetEnumeratorRangeValid()
		{
			var current = IndexBegin;
			while(true)
			{
				if(current.IsIndexValid(_sourceSize))
				{
					yield return current;
				}

				current += Step;
				if(current > IndexEnd)
				{
					break;
				}
			}
		}

		public bool Equals(RangeWindow other)
		{
			return
				IndexBegin == other.IndexBegin &&
				IndexEnd == other.IndexEnd &&
				Step == other.Step;
		}

		public override bool Equals(object obj)
		{
			return obj is RangeWindow other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(IndexBegin, IndexEnd, Step);
		}

		public static bool operator ==(RangeWindow left, RangeWindow right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(RangeWindow left, RangeWindow right)
		{
			return !left.Equals(right);
		}
	}
}
