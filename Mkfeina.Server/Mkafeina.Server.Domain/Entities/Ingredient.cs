using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mkafeina.Server.Domain.Entities
{
	public abstract class Ingredient
	{
		public abstract string Name { get; }

		public abstract char Code { get; }

		public int? Level { get; private set; }

		private int? _minimumLevel;

		public int MinimumLevel {
			get {
				if (_minimumLevel == null)
					_minimumLevel = ((AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>()).IngredientMinimumLevel(Name);
				return _minimumLevel.Value;
			}
		}

		private float? signal;

		public float? Signal {
			get => signal;

			set {
				signal = Available ? value : null;
				var inPercent = (value - EmptyOffset) / (FullOffset - EmptyOffset);
				Level = Available ? (int?)(100 * inPercent) : null;
			}
		}

		public float? EmptyOffset { get; set; }

		public float? FullOffset { get; set; }

		public bool Available { get; set; }

		#region Static Stuff

		public static IEnumerable<string> GetAllExistingIngredientsNames()
		{
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
						where t.IsSubclassOf(typeof(Ingredient))
						select t;
			var names = types.Select(t => t.Name);
			return names;
		}

		public static IDictionary<string, Ingredient> GetAllExistingIngredientsInstances()
		{
			var dict = new Dictionary<string, Ingredient>();
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
						where t.IsSubclassOf(typeof(Ingredient))
						select t;
			types.Select(t =>
			{
				Ingredient obj = (Ingredient)Activator.CreateInstance(t);
				return obj;
			}).ToList().ForEach(obj => dict.Add(obj.Name, obj));
			return dict;
		}

		public static IDictionary<string, char> GetCodeMappingByName()
		{
			var mapping = new Dictionary<string, char>();
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
						where t.IsSubclassOf(typeof(Ingredient))
						select t;
			types.Select(t =>
			{
				Ingredient obj = (Ingredient)Activator.CreateInstance(t);
				return obj;
			}).ToList().ForEach(obj => mapping.Add(obj.Name, obj.Code));
			return mapping;
		}

		public static IDictionary<char, string> GetNameMappingByCode()
		{
			var mapping = new Dictionary<char, string>();
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
						where t.IsSubclassOf(typeof(Ingredient))
						select t;
			types.Select(t =>
			{
				Ingredient obj = (Ingredient)Activator.CreateInstance(t);
				return obj;
			}).ToList().ForEach(obj => mapping.Add(obj.Code, obj.Name));
			return mapping;
		}

		public static char GetCode(string ingredientName) => GetCodeMappingByName()[ingredientName];

		public static string GetName(char ingredientCode) => GetNameMappingByCode()[ingredientCode];

		#endregion Static Stuff
	}
}