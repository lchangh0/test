
namespace test3;

public class CKey
{
    public ushort scanCode;

    public string keyText;
    public char key;
    public char keyShift;
    public char keyLocal;
    public char keyLocalShift;

    public int posX;
    public int posY;

    public string GetKeyText(bool bShift, bool bLocal)
    {
        string strText = null;

        if (bLocal)
        {
            if (bShift)
            {
                if (keyLocalShift != 0)
                    strText = keyLocalShift.ToString();
            }
            else
            {
                if (keyLocal != 0)
                    strText = keyLocal.ToString();
            }
        }
        else
        {
            if (bShift)
            {
                if (keyShift != 0)
                    strText = keyShift.ToString();
            }
            else
            {
                if (key != 0)
                    strText = key.ToString();
            }
        }

        if (strText == null && keyText != null)
            strText = keyText;

        return strText;
    }

}


public class CKeys
{
    List<CKey> keys;

    public CKeys()
    {
        keys = new List<CKey>();
    }

    public CKey GetKey(int x, int y)
    {
        foreach (CKey key in keys)
        {
            if (key.posX == x && key.posY == y)
            {
                return key;
            }
        }

        return null;
    }

    

    public void MakeKorKeys()
    {
        keys.Clear();

        keys.Add(new CKey() { scanCode = 41, key = '`', keyShift = '~', posX = 1, posY = 1 });
        keys.Add(new CKey() { scanCode = 2, key = '1', keyShift = '!', posX = 2, posY = 1 });
        keys.Add(new CKey() { scanCode = 3, key = '2', keyShift = '@', posX = 3, posY = 1 });
        keys.Add(new CKey() { scanCode = 4, key = '3', keyShift = '#', posX = 4, posY = 1 });
        keys.Add(new CKey() { scanCode = 5, key = '4', keyShift = '$', posX = 5, posY = 1 });
        keys.Add(new CKey() { scanCode = 6, key = '5', keyShift = '%', posX = 6, posY = 1 });
        keys.Add(new CKey() { scanCode = 7, key = '6', keyShift = '^', posX = 7, posY = 1 });
        keys.Add(new CKey() { scanCode = 8, key = '7', keyShift = '&', posX = 8, posY = 1 });
        keys.Add(new CKey() { scanCode = 9, key = '8', keyShift = '*', posX = 9, posY = 1 });
        keys.Add(new CKey() { scanCode = 10, key = '9', keyShift = '(', posX = 10, posY = 1 });
        keys.Add(new CKey() { scanCode = 11, key = '0', keyShift = ')', posX = 11, posY = 1 });
        keys.Add(new CKey() { scanCode = 12, key = '-', keyShift = '_', posX = 12, posY = 1 });
        keys.Add(new CKey() { scanCode = 13, key = '=', keyShift = '+', posX = 13, posY = 1 });
        keys.Add(new CKey() { scanCode = 14, keyText = "BackS", posX = 14, posY = 1 }); // 백스페이스

        keys.Add(new CKey() { scanCode = 15, keyText = "Tab", posX = 1, posY = 2 });
        keys.Add(new CKey() { scanCode = 16, key = 'q', keyShift = 'Q', keyLocal = 'ㅂ', keyLocalShift = 'ㅃ', posX = 2, posY = 2 });
        keys.Add(new CKey() { scanCode = 17, key = 'w', keyShift = 'W', keyLocal = 'ㅈ', keyLocalShift = 'ㅉ', posX = 3, posY = 2 });
        keys.Add(new CKey() { scanCode = 18, key = 'e', keyShift = 'E', keyLocal = 'ㄷ', keyLocalShift = 'ㄸ', posX = 4, posY = 2 });
        keys.Add(new CKey() { scanCode = 19, key = 'r', keyShift = 'R', keyLocal = 'ㄱ', keyLocalShift = 'ㄲ', posX = 5, posY = 2 });
        keys.Add(new CKey() { scanCode = 20, key = 't', keyShift = 'T', keyLocal = 'ㅅ', keyLocalShift = 'ㅆ', posX = 6, posY = 2 });
        keys.Add(new CKey() { scanCode = 21, key = 'y', keyShift = 'Y', keyLocal = 'ㅛ', posX = 7, posY = 2 });
        keys.Add(new CKey() { scanCode = 22, key = 'u', keyShift = 'U', keyLocal = 'ㅕ', posX = 8, posY = 2 });
        keys.Add(new CKey() { scanCode = 23, key = 'i', keyShift = 'I', keyLocal = 'ㅑ', posX = 9, posY = 2 });
        keys.Add(new CKey() { scanCode = 24, key = 'o', keyShift = 'O', keyLocal = 'ㅐ', keyLocalShift = 'ㅒ', posX = 10, posY = 2 });
        keys.Add(new CKey() { scanCode = 25, key = 'p', keyShift = 'P', keyLocal = 'ㅔ', keyLocalShift = 'ㅖ', posX = 11, posY = 2 });
        keys.Add(new CKey() { scanCode = 26, key = '[', keyShift = '{', posX = 12, posY = 2 });
        keys.Add(new CKey() { scanCode = 27, key = ']', keyShift = '}', posX = 13, posY = 2 });
        keys.Add(new CKey() { scanCode = 43, key = '\\', keyShift = '|', posX = 14, posY = 2 });

        keys.Add(new CKey() { scanCode = 58, keyText = "Caps", posX = 1, posY = 3 });
        keys.Add(new CKey() { scanCode = 30, key = 'a', keyShift = 'A', keyLocal = 'ㅁ', posX = 2, posY = 3 });
        keys.Add(new CKey() { scanCode = 31, key = 's', keyShift = 'S', keyLocal = 'ㄴ', posX = 3, posY = 3 });
        keys.Add(new CKey() { scanCode = 32, key = 'd', keyShift = 'D', keyLocal = 'ㅇ', posX = 4, posY = 3 });
        keys.Add(new CKey() { scanCode = 33, key = 'f', keyShift = 'F', keyLocal = 'ㄹ', posX = 5, posY = 3 });
        keys.Add(new CKey() { scanCode = 34, key = 'g', keyShift = 'G', keyLocal = 'ㅎ', posX = 6, posY = 3 });
        keys.Add(new CKey() { scanCode = 35, key = 'h', keyShift = 'H', keyLocal = 'ㅗ', posX = 7, posY = 3 });
        keys.Add(new CKey() { scanCode = 36, key = 'j', keyShift = 'J', keyLocal = 'ㅓ', posX = 8, posY = 3 });
        keys.Add(new CKey() { scanCode = 37, key = 'k', keyShift = 'K', keyLocal = 'ㅏ', posX = 9, posY = 3 });
        keys.Add(new CKey() { scanCode = 38, key = 'l', keyShift = 'L', keyLocal = 'ㅣ', posX = 10, posY = 3 });
        keys.Add(new CKey() { scanCode = 39, key = ';', keyShift = ':', posX = 11, posY = 3 });
        keys.Add(new CKey() { scanCode = 40, key = '\'', keyShift = '"', posX = 12, posY = 3 });
        keys.Add(new CKey() { scanCode = 28, keyText = "Enter", posX = 13, posY = 3 });

        keys.Add(new CKey() { scanCode = 42, keyText = "Shift", posX = 1, posY = 4 });
        keys.Add(new CKey() { scanCode = 44, key = 'z', keyShift = 'Z', keyLocal = 'ㅋ', posX = 2, posY = 4 });
        keys.Add(new CKey() { scanCode = 45, key = 'x', keyShift = 'X', keyLocal = 'ㅌ', posX = 3, posY = 4 });
        keys.Add(new CKey() { scanCode = 46, key = 'c', keyShift = 'C', keyLocal = 'ㅊ', posX = 4, posY = 4 });
        keys.Add(new CKey() { scanCode = 47, key = 'v', keyShift = 'V', keyLocal = 'ㅍ', posX = 5, posY = 4 });
        keys.Add(new CKey() { scanCode = 48, key = 'b', keyShift = 'B', keyLocal = 'ㅠ', posX = 6, posY = 4 });
        keys.Add(new CKey() { scanCode = 49, key = 'n', keyShift = 'N', keyLocal = 'ㅜ', posX = 7, posY = 4 });
        keys.Add(new CKey() { scanCode = 50, key = 'm', keyShift = 'M', keyLocal = 'ㅡ', posX = 8, posY = 4 });
        keys.Add(new CKey() { scanCode = 51, key = ',', keyShift = '<', posX = 9, posY = 4 });
        keys.Add(new CKey() { scanCode = 52, key = '.', keyShift = '>', posX = 10, posY = 4 });
        keys.Add(new CKey() { scanCode = 53, key = '/', keyShift = '?', posX = 11, posY = 4 });
        keys.Add(new CKey() { scanCode = 54, keyText = "Shift", posX = 12, posY = 4 });

        keys.Add(new CKey() { scanCode = 29, keyText = "Ctrl", posX = 1, posY = 5 });
        keys.Add(new CKey() { scanCode = 56, keyText = "Alt", posX = 2, posY = 5 });
        keys.Add(new CKey() { scanCode = 219, keyText = "Win", posX = 3, posY = 5 });
        keys.Add(new CKey() { scanCode = 57, keyText = "Space", posX = 4, posY = 5 });
        keys.Add(new CKey() { scanCode = 114, keyText = "한/영", posX = 5, posY = 5 });
        keys.Add(new CKey() { scanCode = 184, keyText = "Alt", posX = 6, posY = 5 });
        keys.Add(new CKey() { scanCode = 157, keyText = "Ctrl", posX = 7, posY = 5 });
    }

}