// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("j+FjpuzVCpbTveD5VLt4uCntV0bM+gm5M/xJBpORDCW2Om336JcKWnY/hIT/U2/xbajRI3I9FruTomT4xZDVx+iGJyKqsTQUg4HS58imXQaUM621GKyiH1y6wUUmRIpe8uas6kr4e1hKd3xzUPwy/I13e3t7f3p54F5ovgDsyV3N5XxBy2f5CS/UD6r4e3V6Svh7cHj4e3t6viJovgUF02R5T3GWt3DJdiVaXekkjp0yYZ+Qjsj6Bw2K5JpdtRWxSTEuko882H3IlLmp5Njj/ZVkkyoHDMK078uYR8wK196Dam6u3daeDK7H9dM9XMdHgwPQWprQtZmiDx3Bo7QeowphXde0x8XnKzR5fbbZhDa/6e00/xVPa7PhITlHejFAsXh5e3p7");
        private static int[] order = new int[] { 10,13,2,12,4,10,7,13,10,9,11,12,13,13,14 };
        private static int key = 122;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
