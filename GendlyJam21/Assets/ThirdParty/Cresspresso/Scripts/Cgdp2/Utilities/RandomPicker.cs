using System;
using System.Collections.Generic;
using System.Linq;

namespace CGDP2.Utilities
{
	public interface IRandomChoice<out TValue>
	{
		TValue value { get; }
		float weight { get; } // must be > 0
	}

	public class RandomChoice<TValue> : IRandomChoice<TValue>
	{
		public RandomChoice(
			TValue value,
			float weight
			)
		{
			if (weight <= 0)
				throw new ArgumentException(nameof(weight) + " must be > 0", nameof(weight));

			this.value = value;
			this.weight = weight;
		}

		public TValue value { get; }
		public float weight { get; }
	}

	public interface IRandomPicker<out TValue>
	{
		IReadOnlyList<IRandomChoice<TValue>> choices { get; } // must not be empty
		IRandomChoice<TValue> Pick();
	}

	public class RandomPicker<TValue> : IRandomPicker<TValue>
	{
		public RandomPicker(
			IReadOnlyList<IRandomChoice<TValue>> choices
			)
		{
			if (choices.Count <= 0)
				throw new ArgumentException("must have at least one choice", nameof(choices));

			this.choices = choices;
		}

		public IReadOnlyList<IRandomChoice<TValue>> choices { get; }

		public IRandomChoice<TValue> Pick()
		{
			float pickedWeight = (float)UnityEngine.Random.Range(0.0f, choices.Sum(burst => burst.weight));
			float currentWeight = 0.0f;
			foreach (var choice in choices)
			{
				currentWeight += choice.weight;
				if (currentWeight >= pickedWeight)
				{
					return choice;
				}
			}
			return choices[choices.Count - 1];
		}
	}
}
