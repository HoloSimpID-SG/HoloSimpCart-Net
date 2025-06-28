using SkiaSharp;

namespace HoloSimpID
{
    /*
    public class FileName
    {
        public static void GenerateInfographicForCart(Cart cart)
        {
            if (cart == null) return;

            // Calculate Height
            Simp cartOwner = cart.cartOwner;
            List<Tuple<Simp, int>> heights = new();
            List<int> columnHeights = new(3);
            foreach(var kvp in cart.cartItems)
            {
                Simp simp = kvp.Key;
                int itemCount = kvp.Value.Count;
                int height =
                    16 + // Top Padding
                    72 + // Title Block
                    16 + // Tilte to cart padding
                    56 * itemCount +
                    16; // Bottom Padding
    
                heights.Add(new Tuple<Simp, int>(simp, height));

                if (simp == cartOwner)
                    columnHeights.Add(height);
                else
                    heights.Add(new Tuple<Simp, int>(simp, height));
            }
            // Sort by Heights
            heights.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            while(heights.Count > 0)
            {

            }

            int height =
                16 + // Top Padding
                72 + // Title Block
                16 + // Tilte to cart padding
                56 * itemCount +
                16; // Bottom Padding
            int width = 320;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
        }
        public static void GenerateSimpBlock(Simp simp, Cart cart)
        {
            Dictionary<Item, uint> cartItems = cart.cartItems[simp];

            int itemCount = cartItems.Count;


            int height = 
                16 + // Top Padding
                72 + // Title Block
                16 + // Tilte to cart padding
                56 * itemCount +
                16; // Bottom Padding
            int width = 320;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true
            };
        }
        public static void GenerateItemBlock(Item item, uint quantity)
        {
            int height =
                16 + // Top Padding
                72 + // Title Block
                16 + // Tilte to cart padding
                56 * itemCount +
                16; // Bottom Padding
            int width = 320;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true
            };
        }
    }*/
}
