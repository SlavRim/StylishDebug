global using Mod = StylishDebug.Mod;

namespace StylishDebug;

public sealed class Mod : Verse.Mod
{
    public static Mod Instance { get; private set; }
    public Mod(ModContentPack content) : base(content)
    {
        Instance = this;
    }

    [DebugAction("General", "Spawn style category things.", allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void SpawnStyleCategoryThings()
    {
        var list = new List<DebugMenuOption>();
        foreach(var styleCategory in DefDatabase<StyleCategoryDef>.AllDefsListForReading)
        {
            var styleCategoryName = styleCategory.defName;
            list.Add(new(styleCategoryName, DebugMenuOptionMode.Action, () =>
            {
                Find.Targeter.BeginTargeting(new TargetingParameters() { canTargetLocations = true }, target =>
                {
                    var cell = target.Cell;
                    var map = Find.CurrentMap;
                    for (var i = 0; i < styleCategory.thingDefStyles.Count; i++)
                    {
                        var thingStyle = styleCategory.thingDefStyles[i];

                        var thingDef = thingStyle.thingDef;
                        if (thingDef is null) Log.Error($"Thing definition is null at element {i} in {styleCategoryName}.");

                        var styleDef = thingStyle.styleDef;
                        if (styleDef is null) Log.Error($"Style definition is null at element {i} in {styleCategoryName}.");

                        if (thingDef is null)
                            continue;

                        var thing = GenSpawn.Spawn(thingDef, cell, map);

                        Log.Message($"Spawning thing '{thingDef.defName}' as '{thing.LabelCap}' with style '{styleDef?.defName}'.");

                        if(styleDef is not null)
                            thing.SetStyleDef(styleDef);
                    }
                });
            }));
        }
        Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
    }
}