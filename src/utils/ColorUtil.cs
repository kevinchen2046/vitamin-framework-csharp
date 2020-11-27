namespace vitamin{
    public class ColorUtil{
        public static string toHex(int value) {
			return "#" + MathUtil.toHex(value);
		}
    }
}