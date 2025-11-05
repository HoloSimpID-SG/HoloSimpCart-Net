using System.Text;
using MMOR.NET.Random;
using MMOR.NET.Utilities;

namespace HoloSimpID {
  public static class UmaScheduler {
    private enum TrackType {
      Turf,
      Dirt,
    }

    private enum LengthCategories {
      Sprint,
      Mile,
      Medium,
      Long,
    }

    private static readonly IReadOnlyDictionary<TrackType, int> TrackTypeWeights =
        new Dictionary<TrackType, int>() {
          [TrackType.Turf] = 165,
          [TrackType.Dirt] = 24,
        };
    private static readonly IReadOnlyDictionary<TrackType, IReadOnlyList<int>> LengthWeights =
        new Dictionary<TrackType, IReadOnlyList<int>>() {
          [TrackType.Turf] = [320, 812, 2140, 678],
          [TrackType.Dirt] = [180, 500, 340, 0],
        };

    enum Direction {
      Left,
      Right,
      Stretch,
    }

    enum Locations {
      Sapporo,
      Hakodate,
      Fukushima,
      Niigata,
      Tokyo,
      Nakayama,
      Chukyo,
      Kyoto,
      Hanshin,
      Kokura,
      Oi,
    }

    enum Extra {
      Outer,
      Inner,
      Outer_Inner,
    }

    private static int HashKey(TrackType type, LengthCategories length) => (1 << (int)type) |
                                                                           1 << ((int)length + 2);

    private static readonly IReadOnlyDictionary<int,
        IReadOnlyList<(Locations location, short length, Direction direction, Extra? extra)>>
        RaceTracks =
            new Dictionary<int, IReadOnlyList<(Locations location, short length,
                                    Direction direction, Extra? extra)>>() {
              [HashKey(TrackType.Turf, LengthCategories.Sprint)] =
                  [
                    (Locations.Sapporo, 1200, Direction.Right, null),
                    (Locations.Hakodate, 1000, Direction.Right, null),
                    (Locations.Hakodate, 1200, Direction.Left, null),
                    (Locations.Fukushima, 1200, Direction.Right, null),
                    (Locations.Niigata, 1400, Direction.Left, Extra.Inner),
                    (Locations.Niigata, 1200, Direction.Left, Extra.Inner),
                    (Locations.Niigata, 1000, Direction.Stretch, null),
                    (Locations.Tokyo, 1400, Direction.Left, null),
                    (Locations.Nakayama, 1200, Direction.Right, Extra.Outer),
                    (Locations.Chukyo, 1400, Direction.Left, null),
                    (Locations.Chukyo, 1200, Direction.Left, null),
                    (Locations.Kyoto, 1400, Direction.Right, Extra.Outer),
                    (Locations.Kyoto, 1400, Direction.Right, Extra.Inner),
                    (Locations.Kyoto, 1200, Direction.Right, Extra.Inner),
                    (Locations.Hanshin, 1400, Direction.Right, Extra.Inner),
                    (Locations.Hanshin, 1200, Direction.Right, Extra.Inner),
                    (Locations.Kokura, 1200, Direction.Right, null),
                  ],
              [HashKey(TrackType.Turf, LengthCategories.Mile)] =
                  [
                    (Locations.Sapporo, 1800, Direction.Right, null),
                    (Locations.Sapporo, 1500, Direction.Right, null),
                    (Locations.Hakodate, 1800, Direction.Right, null),
                    (Locations.Fukushima, 1800, Direction.Right, null),
                    (Locations.Niigata, 1800, Direction.Left, Extra.Outer),
                    (Locations.Niigata, 1600, Direction.Left, Extra.Outer),
                    (Locations.Tokyo, 1800, Direction.Left, null),
                    (Locations.Tokyo, 1600, Direction.Left, null),
                    (Locations.Nakayama, 1800, Direction.Right, Extra.Inner),
                    (Locations.Nakayama, 1600, Direction.Right, Extra.Outer),
                    (Locations.Chukyo, 1600, Direction.Left, null),
                    (Locations.Hanshin, 1800, Direction.Right, Extra.Outer),
                    (Locations.Hanshin, 1600, Direction.Right, Extra.Outer),
                    (Locations.Kokura, 1800, Direction.Right, null),
                  ],
              [HashKey(TrackType.Turf, LengthCategories.Medium)] =
                  [
                    (Locations.Sapporo, 2000, Direction.Right, null),
                    (Locations.Hakodate, 2000, Direction.Right, null),
                    (Locations.Fukushima, 2000, Direction.Right, null),
                    (Locations.Niigata, 2400, Direction.Left, Extra.Inner),
                    (Locations.Niigata, 2200, Direction.Left, Extra.Inner),
                    (Locations.Niigata, 2000, Direction.Left, Extra.Outer),
                    (Locations.Niigata, 2000, Direction.Left, Extra.Inner),
                    (Locations.Tokyo, 2400, Direction.Left, null),
                    (Locations.Tokyo, 2300, Direction.Left, null),
                    (Locations.Tokyo, 2000, Direction.Left, null),
                    (Locations.Nakayama, 2200, Direction.Right, Extra.Outer),
                    (Locations.Nakayama, 2000, Direction.Right, Extra.Inner),
                    (Locations.Chukyo, 2200, Direction.Left, null),
                    (Locations.Chukyo, 2000, Direction.Left, null),
                    (Locations.Kyoto, 2400, Direction.Right, Extra.Outer),
                    (Locations.Kyoto, 2200, Direction.Right, Extra.Outer),
                    (Locations.Kyoto, 2000, Direction.Right, Extra.Inner),
                    (Locations.Hanshin, 2400, Direction.Right, Extra.Outer),
                    (Locations.Hanshin, 2200, Direction.Right, Extra.Inner),
                    (Locations.Hanshin, 2000, Direction.Right, Extra.Inner),
                    (Locations.Kokura, 2000, Direction.Right, null),
                  ],
              [HashKey(TrackType.Turf, LengthCategories.Long)] =
                  [
                    (Locations.Sapporo, 2600, Direction.Right, null),
                    (Locations.Hakodate, 2600, Direction.Right, null),
                    (Locations.Fukushima, 2600, Direction.Right, null),
                    (Locations.Tokyo, 3400, Direction.Left, null),
                    (Locations.Tokyo, 2500, Direction.Left, null),
                    (Locations.Nakayama, 3600, Direction.Right, Extra.Inner),
                    (Locations.Nakayama, 2500, Direction.Right, Extra.Inner),
                    (Locations.Kyoto, 3200, Direction.Right, Extra.Outer),
                    (Locations.Kyoto, 3000, Direction.Right, Extra.Outer),
                    (Locations.Hanshin, 3200, Direction.Right, Extra.Outer_Inner),
                    (Locations.Hanshin, 3000, Direction.Right, Extra.Inner),
                    (Locations.Hanshin, 2600, Direction.Right, Extra.Outer),
                    (Locations.Kokura, 2600, Direction.Right, null),
                  ],
              [HashKey(TrackType.Dirt, LengthCategories.Sprint)] =
                  [
                    (Locations.Fukushima, 1150, Direction.Right, null),
                    (Locations.Niigata, 1200, Direction.Left, null),
                    (Locations.Tokyo, 1400, Direction.Left, null),
                    (Locations.Tokyo, 1300, Direction.Left, null),
                    (Locations.Nakayama, 1200, Direction.Right, null),
                    (Locations.Chukyo, 1400, Direction.Left, null),
                    (Locations.Kyoto, 1400, Direction.Right, null),
                    (Locations.Kyoto, 1200, Direction.Right, null),
                    (Locations.Hanshin, 1400, Direction.Right, null),
                    (Locations.Oi, 1200, Direction.Right, null),
                  ],
              [HashKey(TrackType.Dirt, LengthCategories.Mile)] =
                  [
                    (Locations.Sapporo, 1700, Direction.Right, null),
                    (Locations.Hakodate, 1700, Direction.Right, null),
                    (Locations.Fukushima, 1700, Direction.Right, null),
                    (Locations.Niigata, 1800, Direction.Left, null),
                    (Locations.Tokyo, 1600, Direction.Left, null),
                    (Locations.Nakayama, 1800, Direction.Right, null),
                    (Locations.Chukyo, 1800, Direction.Left, null),
                    (Locations.Kyoto, 1800, Direction.Right, null),
                    (Locations.Kokura, 1700, Direction.Right, null),
                    (Locations.Oi, 1800, Direction.Right, null),
                  ],
              [HashKey(TrackType.Dirt, LengthCategories.Medium)] =
                  [
                    (Locations.Tokyo, 2100, Direction.Left, null),
                    (Locations.Kyoto, 1900, Direction.Right, null),
                    (Locations.Hanshin, 2000, Direction.Right, null),
                    (Locations.Oi, 2000, Direction.Right, null),
                  ],
            };

    private static int GetWeightedIndex<T>(IReadOnlyList<T> weights)
        where T : struct, IConvertible {
      int totalWeight = weights.Sum(x => x.ToInt32(null));

      int prob    = PCG.global.NextInt(0, totalWeight);
      totalWeight = 0;

      for (var i = 0; i < weights.Count; i++) {
        totalWeight += weights[i].ToInt32(null);
        if (prob < totalWeight)
          return i;
      }
      return -1;
    }

    private static T GetWeightedKey<T>(IReadOnlyDictionary<T, int> weights)
        where T : struct {
      int totalWeight = weights.Sum(x => x.Value);

      int prob    = PCG.global.NextInt(0, totalWeight);
      totalWeight = 0;

      foreach ((T key, int weight) in weights) {
        totalWeight += weight;
        if (prob < totalWeight)
          return key;
      }
      return default;
    }

    public static string CreateSchedule(int num = 3) {
      StringBuilder strResult             = new();
      Dictionary<int, uint> raceBanHelper = [];
      for (var i = 0; i < Math.Clamp(num, 1, 5); i++) {
        int key;
        TrackType trackType;
        LengthCategories lengthCategory;
        while (true) {
          trackType      = GetWeightedKey(TrackTypeWeights);
          lengthCategory = (LengthCategories)GetWeightedIndex(LengthWeights[trackType]);
          key            = HashKey(trackType, lengthCategory);

          if (raceBanHelper.TryGetValue(key, out uint count)) {
            if (count >= 2)
              continue;
            raceBanHelper[key] = count + 1;
            break;
          }
          raceBanHelper.Add(key, 1);
          break;
        }
        var randomTrack = RaceTracks[key][PCG.global.NextInt(0, RaceTracks[key].Count)];
        strResult.Append(randomTrack.location)
            .Append(' ')
            .Append(trackType)
            .Append(' ')
            .Append($"{randomTrack.length}m ")
            .Append($"({lengthCategory}) ")
            .Append(randomTrack.direction)
            .Append(randomTrack.extra.HasValue
                        ? $" / {randomTrack.extra.ToString().Replace('_', '\u2192')}"
                        : string.Empty)
            .AppendLine();
      }
      return strResult.ToString();
    }
  }
}
