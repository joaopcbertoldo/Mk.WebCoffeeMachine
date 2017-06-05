using Mkafeina.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mkafeina.Simulator.Constants;

namespace Mkafeina.CoffeeMachineSimulator
{
	public static class CookBookExtension
	{
		private static LinkedList<KeyValuePair<string, Recipe>> __extensionRecipes;

		private static LinkedListNode<KeyValuePair<string, Recipe>> __selectedRecipe;

		public static event Action<string> SelectedRecipeChangeEvent;

		public static string SelectedRecipeName(this CookBook cookBook)
		{
			if (__extensionRecipes == null || __selectedRecipe == null)
				cookBook.UpdateExtension();
			return __selectedRecipe.Value.Key;
		}

		public static Recipe SelectedRecipe(this CookBook cookBook)
		{
			if (__extensionRecipes == null || __selectedRecipe == null)
				cookBook.UpdateExtension();
			return __selectedRecipe.Value.Value;
		}

		public static void NextRecipe(this CookBook cookBook)
		{
			if (!cookBook.ExtensionRecipesIsUpToDate())
			{
				cookBook.UpdateExtension();
				return;
			}
			__selectedRecipe = __selectedRecipe.NextOrFirst();
			SelectedRecipeChangeEvent?.Invoke(PANEL_LINE_SELECTED_RECIPE);
		}

		public static void PreviousRecipe(this CookBook cookBook)
		{
			if (!cookBook.ExtensionRecipesIsUpToDate())
			{
				cookBook.UpdateExtension();
				return;
			}
			__selectedRecipe = __selectedRecipe.PreviousOrLast();
			SelectedRecipeChangeEvent?.Invoke(PANEL_LINE_SELECTED_RECIPE);
		}

		private static bool ExtensionRecipesIsUpToDate(this CookBook cookbook)
		{
			if (__extensionRecipes == null)
				return false;
			var recipesFromCookBok = cookbook.AllRecipesNames;
			return recipesFromCookBok.All(name => __extensionRecipes.Any(kv => kv.Key == name)) &&
				   __extensionRecipes.All(kv => recipesFromCookBok.Any(name => name == kv.Key));
		}

		private static void UpdateExtension(this CookBook cookbook)
		{
			__extensionRecipes = new LinkedList<KeyValuePair<string, Recipe>>(cookbook.AllRecipes());
			__selectedRecipe = __extensionRecipes.First;
			SelectedRecipeChangeEvent?.Invoke(PANEL_LINE_SELECTED_RECIPE);
		}

		public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
			=> current.Next ?? current.List.First;

		public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
			=> current.Previous ?? current.List.Last;
	}
}