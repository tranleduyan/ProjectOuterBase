using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class colorSwap
{
    public Color fromColor;
    public Color toColor;

    public colorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}
public class ApplyCharacterCustomization : MonoBehaviour
{
    //Input Textures
    [Header("Base Textures")]
    [SerializeField] private Texture2D maleCharacterBaseTexture = null;
    [SerializeField] private Texture2D femaleCharacterBaseTexture = null;
    [SerializeField] private Texture2D hairBaseTexture = null;
    private Texture2D characterBaseTexture;

    //Created/Output Textures
    [Header("Output Base Texture To Be Used For Animation")]
    [SerializeField] private Texture2D characterBaseCustomized = null;
    [SerializeField] private Texture2D hairCustomized = null;

    //Select Sex
    [Header("Select Sex: 0 = Male, 1 = Female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    //Select Skin Type
    [Header("Select Skin Type")]
    [Range(0, 2)]
    [SerializeField] private int inputSkinType = 0;

    //Select Hair Style
    [Header("Select Hair Style")]
    [Range(0, 2)]
    [SerializeField] private int inputHairStyleNo = 0;
    
    //Select hair color
    [SerializeField] private Color inputHairColor = Color.black;

    //SelectShirtColor
    [SerializeField] private Color inputShirtColor = Color.green;

    //private Facing[,] bodyFacingArray;
    //private Vector2Int[,] bodyShirtOffsetArray;

    //Dimension
   //private int bodyRows = 9;
    private int bodyColumns = 28;
    private int characterSpriteWidth = 32;
    //private int characterSpriteHeight = 32;

    //Hair texture
    private int hairTextureWidth = 32;
    private int hairTextureHeight = 3008;
    private int hairStylesInSpriteWidth = 3;


    private List<colorSwap> colorSwapList;

    //Target color for arms for color replacement
    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);//darkest
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255);//next darkest
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255);//lightest

    private Color32 skinTargetColor1 = new Color32(143, 77, 87, 255);
    private Color32 skinTargetColor2 = new Color32(189, 106, 98, 255);
    private Color32 skinTargetColor3 = new Color32(255, 174, 112, 255);
    private Color32 skinTargetColor4 = new Color32(220, 146, 89, 255);

    private void Awake()
    {
        //initialize color swap list
        colorSwapList = new List<colorSwap>();

        //process customization
        ProcessCustomization();
    }

    private void ProcessCustomization()
    {
        ProcessGender();

        ProcessShirt();

        ProcessHair();

        ProcessSkin();

        MergeCustomization();
    }

    private void ProcessGender()
    {
        //Set Base sprite sheet by gender
        if(inputSex == 0)
        {
            characterBaseTexture = maleCharacterBaseTexture;
        }
        else if(inputSex == 1)
        {
            characterBaseTexture = femaleCharacterBaseTexture;
        }

        //Get base pxels
        Color[] characterBasePixels = characterBaseTexture.GetPixels();

        //Set Changed base pixels
        characterBaseCustomized.SetPixels(characterBasePixels);
        characterBaseCustomized.Apply();
    }

    private void ProcessShirt()
    {
        //Get Shirt pixels to recolor
        Color[] characterShirtPixels = characterBaseTexture.GetPixels(1856, 0, 896, characterBaseTexture.height);

        //change shirt colors
        TintPixelColors(characterShirtPixels, inputShirtColor);

        //SetChanged shirt pixels
        characterBaseCustomized.SetPixels(1856, 0, 896, characterBaseTexture.height, characterShirtPixels);

        //apply texture changes
        characterBaseCustomized.Apply();
    }

    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        //Loop through pixels to tint
        for(int i = 0; i < basePixelArray.Length; i++)
        {
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
        }
    }

    private void ProcessHair()
    {
        //Created Selected Hair Texture
        AddHairToTexture(inputHairStyleNo);

        //Get Hair pixels to recolor
        Color[] characterSelectedHairPixels = hairCustomized.GetPixels();

        //Tint hair pixels
        TintPixelColors(characterSelectedHairPixels, inputHairColor);

        hairCustomized.SetPixels(characterSelectedHairPixels);
        hairCustomized.Apply();
    }

    private void AddHairToTexture(int hairStyleNo)
    {
        //Calculate coordinates for hair pixels
        int y = (hairStyleNo / hairStylesInSpriteWidth) * hairTextureHeight;
        int x = (hairStyleNo % hairStylesInSpriteWidth) * hairTextureWidth;

        //Get hair pixels
        Color[] hairPixels = hairBaseTexture.GetPixels(x, y, hairTextureWidth, hairTextureHeight);

        //Apply SelectedHair pixels to texture
        hairCustomized.SetPixels(hairPixels);
        hairCustomized.Apply();
    }

    private void ProcessSkin()
    {
        //Get skin pixels to recolor
        Color[] characterPixelsToRecolor = characterBaseCustomized.GetPixels(0, 0, 1856, characterBaseTexture.height);

        //Populate Skin Color Swap list
        PopulateSkinColorSwapList(inputSkinType);

        //Change Skin Colors
        ChangePixelColors(characterPixelsToRecolor, colorSwapList);

        //SetRecolor pixels
        characterBaseCustomized.SetPixels(0, 0, 1856, characterBaseTexture.height, characterPixelsToRecolor);

        //Apply Texture changes
        characterBaseCustomized.Apply();
    }

    private void PopulateSkinColorSwapList(int skinType)
    {
        //Clear color swap list
        colorSwapList.Clear();

        //Skin replace ment colors, switch on skin type
        switch (skinType)
        {
            case 0:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
            case 1:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(154, 107, 71, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(202, 148, 106, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(255, 195, 150, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(227, 170, 128, 255)));
                break;
            case 2:
                colorSwapList.Add(new colorSwap(skinTargetColor1, new Color32(117, 63, 22, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor2, new Color32(166, 93, 37, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor3, new Color32(215, 137, 78, 255)));
                colorSwapList.Add(new colorSwap(skinTargetColor4, new Color32(182, 111, 57, 255)));
                break;
            default:
                colorSwapList.Add(new colorSwap(skinTargetColor1, skinTargetColor1));
                colorSwapList.Add(new colorSwap(skinTargetColor2, skinTargetColor2));
                colorSwapList.Add(new colorSwap(skinTargetColor3, skinTargetColor3));
                colorSwapList.Add(new colorSwap(skinTargetColor4, skinTargetColor4));
                break;
        }
    }

    private void ChangePixelColors(Color[] baseArray, List<colorSwap> colorSwapList)
    {
        for (int i = 0; i < baseArray.Length; i++)
        {
            //loop through color swap list
            if (colorSwapList.Count > 0)
            {
                for (int j = 0; j < colorSwapList.Count; j++)
                {
                    if (isSameColor(baseArray[i], colorSwapList[j].fromColor))
                    {
                        baseArray[i] = colorSwapList[j].toColor;
                    }
                }
            }
        }
    }

    private bool isSameColor(Color color1, Color color2)
    {
        if ((color1 == color2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MergeCustomization()
    {
        //Shirt pixels
        Color[] characterShirtPixelsSelection = characterBaseCustomized.GetPixels(1856, 0, 896, characterBaseTexture.height);

        //bodyPixel
        Color[] characterBodyPixels = characterBaseCustomized.GetPixels(0, 0, bodyColumns * characterSpriteWidth, characterBaseTexture.height);

        MergeColorArray(characterBodyPixels, characterShirtPixelsSelection);

        //paste merged pixels
        characterBaseCustomized.SetPixels(0, 0, bodyColumns * characterSpriteWidth, characterBaseTexture.height, characterBodyPixels);

        //Apply texture changes
        characterBaseCustomized.Apply();
    }


    private void MergeColorArray(Color[] baseArray, Color[] mergeArray)
    {
        for(int i = 0; i < baseArray.Length; i++)
        {
            if(mergeArray[i].a > 0)
            {
                //merge array has color
                if(mergeArray[i].a >= 1)
                {
                    //fully replace
                    baseArray[i] = mergeArray[i];
                }
                else
                {
                    //interpolate colors
                    float alpha = mergeArray[i].a;

                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;

                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;

                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;

                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }

/*
    private void ProcessTheShirt()
    {
        //initialize body facing shirt array
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        //populate body facing shirt array
        PopulateBodyFacingArray();

        //initialize body shirt offset array
        bodyShirtOffsetArray = new Vector2Int[bodyColumns, bodyRows];

        //Populate body shirt offset array
        PopulateBodyShirtOffsetArray();

        //Created selected shirt Texture
        AddShirtToTexture(inputShirtStyleNo);

        //Apply shirt Texture to base
        ApplyShirtTextureToBase();
    }

    private void PopulateBodyFacingArray()
    {
        #region row 0 none
        bodyFacingArray[0, 0] = Facing.none;
        bodyFacingArray[1, 0] = Facing.none;
        bodyFacingArray[2, 0] = Facing.none;
        bodyFacingArray[3, 0] = Facing.none;
        bodyFacingArray[4, 0] = Facing.none;
        bodyFacingArray[5, 0] = Facing.none;
        bodyFacingArray[6, 0] = Facing.none;
        bodyFacingArray[7, 0] = Facing.none;
        bodyFacingArray[8, 0] = Facing.none;
        bodyFacingArray[9, 0] = Facing.none;
        bodyFacingArray[10, 0] = Facing.none;
        bodyFacingArray[11, 0] = Facing.none;
        bodyFacingArray[12, 0] = Facing.none;
        bodyFacingArray[13, 0] = Facing.none;
        bodyFacingArray[14, 0] = Facing.none;
        bodyFacingArray[15, 0] = Facing.none;
        bodyFacingArray[16, 0] = Facing.none;
        bodyFacingArray[17, 0] = Facing.none;
        bodyFacingArray[18, 0] = Facing.none;
        bodyFacingArray[19, 0] = Facing.none;
        bodyFacingArray[20, 0] = Facing.none;
        bodyFacingArray[21, 0] = Facing.none;
        bodyFacingArray[22, 0] = Facing.none;
        bodyFacingArray[23, 0] = Facing.none;
        bodyFacingArray[24, 0] = Facing.none;
        bodyFacingArray[25, 0] = Facing.none;
        bodyFacingArray[26, 0] = Facing.none;
        bodyFacingArray[27, 0] = Facing.none;
        #endregion row 0 none

        #region row 1 sit
        bodyFacingArray[0, 1] = Facing.down;
        bodyFacingArray[1, 1] = Facing.none;
        bodyFacingArray[2, 1] = Facing.none;
        bodyFacingArray[3, 1] = Facing.none;
        bodyFacingArray[4, 1] = Facing.none;
        bodyFacingArray[5, 1] = Facing.none;
        bodyFacingArray[6, 1] = Facing.none;
        bodyFacingArray[7, 1] = Facing.none;
        bodyFacingArray[8, 1] = Facing.none;
        bodyFacingArray[9, 1] = Facing.none;
        bodyFacingArray[10, 1] = Facing.none;
        bodyFacingArray[11, 1] = Facing.none;
        bodyFacingArray[12, 1] = Facing.none;
        bodyFacingArray[13, 1] = Facing.none;
        bodyFacingArray[14, 1] = Facing.none;
        bodyFacingArray[15, 1] = Facing.none;
        bodyFacingArray[16, 1] = Facing.none;
        bodyFacingArray[17, 1] = Facing.none;
        bodyFacingArray[18, 1] = Facing.none;
        bodyFacingArray[19, 1] = Facing.none;
        bodyFacingArray[20, 1] = Facing.none;
        bodyFacingArray[21, 1] = Facing.none;
        bodyFacingArray[22, 1] = Facing.none;
        bodyFacingArray[23, 1] = Facing.none;
        bodyFacingArray[24, 1] = Facing.none;
        bodyFacingArray[25, 1] = Facing.none;
        bodyFacingArray[26, 1] = Facing.none;
        bodyFacingArray[27, 1] = Facing.none;
        #endregion row 1 sit

        #region row 2 swinging
        bodyFacingArray[0, 2] = Facing.down;
        bodyFacingArray[1, 2] = Facing.down;
        bodyFacingArray[2, 2] = Facing.down;
        bodyFacingArray[3, 2] = Facing.up;
        bodyFacingArray[4, 2] = Facing.up;
        bodyFacingArray[5, 2] = Facing.up;
        bodyFacingArray[6, 2] = Facing.right;
        bodyFacingArray[7, 2] = Facing.right;
        bodyFacingArray[8, 2] = Facing.right;
        bodyFacingArray[9, 2] = Facing.left;
        bodyFacingArray[10, 2] = Facing.left;
        bodyFacingArray[11, 2] = Facing.left;
        bodyFacingArray[12, 2] = Facing.none;
        bodyFacingArray[13, 2] = Facing.none;
        bodyFacingArray[14, 2] = Facing.none;
        bodyFacingArray[15, 2] = Facing.none;
        bodyFacingArray[16, 2] = Facing.none;
        bodyFacingArray[17, 2] = Facing.none;
        bodyFacingArray[18, 2] = Facing.none;
        bodyFacingArray[19, 2] = Facing.none;
        bodyFacingArray[20, 2] = Facing.none;
        bodyFacingArray[21, 2] = Facing.none;
        bodyFacingArray[22, 2] = Facing.none;
        bodyFacingArray[23, 2] = Facing.none;
        bodyFacingArray[24, 2] = Facing.none;
        bodyFacingArray[25, 2] = Facing.none;
        bodyFacingArray[26, 2] = Facing.none;
        bodyFacingArray[27, 2] = Facing.none;
        #endregion row 2 swinging

        #region row 3 smashing
        bodyFacingArray[0, 3] = Facing.down;
        bodyFacingArray[1, 3] = Facing.down;
        bodyFacingArray[2, 3] = Facing.down;
        bodyFacingArray[3, 3] = Facing.up;
        bodyFacingArray[4, 3] = Facing.up;
        bodyFacingArray[5, 3] = Facing.up;
        bodyFacingArray[6, 3] = Facing.right;
        bodyFacingArray[7, 3] = Facing.right;
        bodyFacingArray[8, 3] = Facing.right;
        bodyFacingArray[9, 3] = Facing.left;
        bodyFacingArray[10, 3] = Facing.left;
        bodyFacingArray[11, 3] = Facing.left;
        bodyFacingArray[12, 3] = Facing.none;
        bodyFacingArray[13, 3] = Facing.none;
        bodyFacingArray[14, 3] = Facing.none;
        bodyFacingArray[15, 3] = Facing.none;
        bodyFacingArray[16, 3] = Facing.none;
        bodyFacingArray[17, 3] = Facing.none;
        bodyFacingArray[18, 3] = Facing.none;
        bodyFacingArray[19, 3] = Facing.none;
        bodyFacingArray[20, 3] = Facing.none;
        bodyFacingArray[21, 3] = Facing.none;
        bodyFacingArray[22, 3] = Facing.none;
        bodyFacingArray[23, 3] = Facing.none;
        bodyFacingArray[24, 3] = Facing.none;
        bodyFacingArray[25, 3] = Facing.none;
        bodyFacingArray[26, 3] = Facing.none;
        bodyFacingArray[27, 3] = Facing.none;
        #endregion row 3 smashing

        #region row 4 sleep
        bodyFacingArray[0, 4] = Facing.down;
        bodyFacingArray[1, 4] = Facing.none;
        bodyFacingArray[2, 4] = Facing.none;
        bodyFacingArray[3, 4] = Facing.none;
        bodyFacingArray[4, 4] = Facing.none;
        bodyFacingArray[5, 4] = Facing.none;
        bodyFacingArray[6, 4] = Facing.none;
        bodyFacingArray[7, 4] = Facing.none;
        bodyFacingArray[8, 4] = Facing.none;
        bodyFacingArray[9, 4] = Facing.none;
        bodyFacingArray[10, 4] = Facing.none;
        bodyFacingArray[11, 4] = Facing.none;
        bodyFacingArray[12, 4] = Facing.none;
        bodyFacingArray[13, 4] = Facing.none;
        bodyFacingArray[14, 4] = Facing.none;
        bodyFacingArray[15, 4] = Facing.none;
        bodyFacingArray[16, 4] = Facing.none;
        bodyFacingArray[17, 4] = Facing.none;
        bodyFacingArray[18, 4] = Facing.none;
        bodyFacingArray[19, 4] = Facing.none;
        bodyFacingArray[20, 4] = Facing.none;
        bodyFacingArray[21, 4] = Facing.none;
        bodyFacingArray[22, 4] = Facing.none;
        bodyFacingArray[23, 4] = Facing.none;
        bodyFacingArray[24, 4] = Facing.none;
        bodyFacingArray[25, 4] = Facing.none;
        bodyFacingArray[26, 4] = Facing.none;
        bodyFacingArray[27, 4] = Facing.none;
        #endregion row 4 sleep

        #region row 5 lifting
        bodyFacingArray[0, 5] = Facing.down;
        bodyFacingArray[1, 5] = Facing.down;
        bodyFacingArray[2, 5] = Facing.down;
        bodyFacingArray[3, 5] = Facing.up;
        bodyFacingArray[4, 5] = Facing.up;
        bodyFacingArray[5, 5] = Facing.up;
        bodyFacingArray[6, 5] = Facing.right;
        bodyFacingArray[7, 5] = Facing.right;
        bodyFacingArray[8, 5] = Facing.right;
        bodyFacingArray[9, 5] = Facing.left;
        bodyFacingArray[10, 5] = Facing.left;
        bodyFacingArray[11, 5] = Facing.left;
        bodyFacingArray[12, 5] = Facing.none;
        bodyFacingArray[13, 5] = Facing.none;
        bodyFacingArray[14, 5] = Facing.none;
        bodyFacingArray[15, 5] = Facing.none;
        bodyFacingArray[16, 5] = Facing.none;
        bodyFacingArray[17, 5] = Facing.none;
        bodyFacingArray[18, 5] = Facing.none;
        bodyFacingArray[19, 5] = Facing.none;
        bodyFacingArray[20, 5] = Facing.none;
        bodyFacingArray[21, 5] = Facing.none;
        bodyFacingArray[22, 5] = Facing.none;
        bodyFacingArray[23, 5] = Facing.none;
        bodyFacingArray[24, 5] = Facing.none;
        bodyFacingArray[25, 5] = Facing.none;
        bodyFacingArray[26, 5] = Facing.none;
        bodyFacingArray[27, 5] = Facing.none;
        #endregion row 5 lifting

        #region row 6 carry
        bodyFacingArray[0, 6] = Facing.down;
        bodyFacingArray[1, 6] = Facing.down;
        bodyFacingArray[2, 6] = Facing.down;
        bodyFacingArray[3, 6] = Facing.up;
        bodyFacingArray[4, 6] = Facing.up;
        bodyFacingArray[5, 6] = Facing.up;
        bodyFacingArray[6, 6] = Facing.right;
        bodyFacingArray[7, 6] = Facing.right;
        bodyFacingArray[8, 6] = Facing.right;
        bodyFacingArray[9, 6] = Facing.left;
        bodyFacingArray[10, 6] = Facing.left;
        bodyFacingArray[11, 6] = Facing.left;
        bodyFacingArray[12, 6] = Facing.down;
        bodyFacingArray[13, 6] = Facing.down;
        bodyFacingArray[14, 6] = Facing.down;
        bodyFacingArray[15, 6] = Facing.down;
        bodyFacingArray[16, 6] = Facing.up;
        bodyFacingArray[17, 6] = Facing.up;
        bodyFacingArray[18, 6] = Facing.up;
        bodyFacingArray[19, 6] = Facing.up;
        bodyFacingArray[20, 6] = Facing.right;
        bodyFacingArray[21, 6] = Facing.right;
        bodyFacingArray[22, 6] = Facing.right;
        bodyFacingArray[23, 6] = Facing.right;
        bodyFacingArray[24, 6] = Facing.left;
        bodyFacingArray[25, 6] = Facing.left;
        bodyFacingArray[26, 6] = Facing.left;
        bodyFacingArray[27, 6] = Facing.left;
        #endregion row 6 carry

        #region row 7 walk
        bodyFacingArray[0, 7] = Facing.down;
        bodyFacingArray[1, 7] = Facing.down;
        bodyFacingArray[2, 7] = Facing.down;
        bodyFacingArray[3, 7] = Facing.down;
        bodyFacingArray[4, 7] = Facing.up;
        bodyFacingArray[5, 7] = Facing.up;
        bodyFacingArray[6, 7] = Facing.up;
        bodyFacingArray[7, 7] = Facing.up;
        bodyFacingArray[8, 7] = Facing.right;
        bodyFacingArray[9, 7] = Facing.right;
        bodyFacingArray[10, 7] = Facing.right;
        bodyFacingArray[11, 7] = Facing.right;
        bodyFacingArray[12, 7] = Facing.left;
        bodyFacingArray[13, 7] = Facing.left;
        bodyFacingArray[14, 7] = Facing.left;
        bodyFacingArray[15, 7] = Facing.left;
        bodyFacingArray[16, 7] = Facing.none;
        bodyFacingArray[17, 7] = Facing.none;
        bodyFacingArray[18, 7] = Facing.none;
        bodyFacingArray[19, 7] = Facing.none;
        bodyFacingArray[20, 7] = Facing.none;
        bodyFacingArray[21, 7] = Facing.none;
        bodyFacingArray[22, 7] = Facing.none;
        bodyFacingArray[23, 7] = Facing.none;
        bodyFacingArray[24, 7] = Facing.none;
        bodyFacingArray[25, 7] = Facing.none;
        bodyFacingArray[26, 7] = Facing.none;
        bodyFacingArray[27, 7] = Facing.none;
        #endregion row 7 walk

        #region row 8 idle
        bodyFacingArray[0, 8] = Facing.down;
        bodyFacingArray[1, 8] = Facing.down;
        bodyFacingArray[2, 8] = Facing.down;
        bodyFacingArray[3, 8] = Facing.up;
        bodyFacingArray[4, 8] = Facing.up;
        bodyFacingArray[5, 8] = Facing.up;
        bodyFacingArray[6, 8] = Facing.right;
        bodyFacingArray[7, 8] = Facing.right;
        bodyFacingArray[8, 8] = Facing.right;
        bodyFacingArray[9, 8] = Facing.left;
        bodyFacingArray[10, 8] = Facing.left;
        bodyFacingArray[11, 8] = Facing.left;
        bodyFacingArray[12, 8] = Facing.none;
        bodyFacingArray[13, 8] = Facing.none;
        bodyFacingArray[14, 8] = Facing.none;
        bodyFacingArray[15, 8] = Facing.none;
        bodyFacingArray[16, 8] = Facing.none;
        bodyFacingArray[17, 8] = Facing.none;
        bodyFacingArray[18, 8] = Facing.none;
        bodyFacingArray[19, 8] = Facing.none;
        bodyFacingArray[20, 8] = Facing.none;
        bodyFacingArray[21, 8] = Facing.none;
        bodyFacingArray[22, 8] = Facing.none;
        bodyFacingArray[23, 8] = Facing.none;
        bodyFacingArray[24, 8] = Facing.none;
        bodyFacingArray[25, 8] = Facing.none;
        bodyFacingArray[26, 8] = Facing.none;
        bodyFacingArray[27, 8] = Facing.none;
        #endregion row 8 idle

    }

    private void PopulateBodyShirtOffsetArray()
    {
        #region row 0 none
        bodyShirtOffsetArray[0, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[1, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[6, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[7, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[8, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[9, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[10, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[11, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[12, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 0] = new Vector2Int(99, 99);
        #endregion row 0 none

        #region row 1 sit
        bodyShirtOffsetArray[0, 1] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[6, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[7, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[8, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[9, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[10, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[11, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[12, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 1] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 1] = new Vector2Int(99, 99);
        #endregion row 1 sit

        #region row 2 swing
        bodyShirtOffsetArray[0, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[2, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[3, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[4, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[5, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[6, 2] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[7, 2] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[8, 2] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[9, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[10, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[11, 2] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[12, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 0] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 0] = new Vector2Int(99, 99);
        #endregion row 2 swing

        #region row 3 smash
        bodyShirtOffsetArray[0, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[2, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[3, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[4, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[5, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[6, 3] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[7, 3] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[8, 3] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[9, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[10, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[11, 3] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[12, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 3] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 3] = new Vector2Int(99, 99);
        #endregion row 3 smash

        #region row 4 sleep
        bodyShirtOffsetArray[0, 4] = new Vector2Int(12, 9);
        bodyShirtOffsetArray[1, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[2, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[3, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[4, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[5, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[6, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[7, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[8, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[9, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[10, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[11, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[12, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 4] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 4] = new Vector2Int(99, 99);
        #endregion row 4 sleep

        #region row 5 lift
        bodyShirtOffsetArray[0, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[2, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[3, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[4, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[5, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[6, 5] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[7, 5] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[8, 5] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[9, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[10, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[11, 5] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[12, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 5] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 5] = new Vector2Int(99, 99);
        #endregion row 5 lift

        #region row 6 carry
        bodyShirtOffsetArray[0, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[2, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[3, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[4, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[5, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[6, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[7, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[8, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[9, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[10, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[11, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[12, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[13, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[14, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[15, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[16, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[17, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[18, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[19, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[20, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[21, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[22, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[23, 6] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[24, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[25, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[26, 6] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[27, 6] = new Vector2Int(12, 8);
        #endregion row 6 carry

        #region row 7 walk
        bodyShirtOffsetArray[0, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[2, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[3, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[4, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[5, 7] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[6, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[7, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[8, 7] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[9, 7] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[10, 7] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[11, 7] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[12, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[13, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[14, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[15, 7] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[16, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 7] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 7] = new Vector2Int(99, 99);
        #endregion row 7 walk

        #region row 8 idle
        bodyShirtOffsetArray[0, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[1, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[2, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[3, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[4, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[5, 8] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[6, 8] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[7, 8] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[8, 8] = new Vector2Int(10, 8);
        bodyShirtOffsetArray[9, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[10, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[11, 8] = new Vector2Int(12, 8);
        bodyShirtOffsetArray[12, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[13, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[14, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[15, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[16, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[17, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[18, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[19, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[20, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[21, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[22, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[23, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[24, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[25, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[26, 8] = new Vector2Int(99, 99);
        bodyShirtOffsetArray[27, 8] = new Vector2Int(99, 99);
        #endregion row 8 idle

    }

    private void AddShirtToTexture(int shirtStyleNo)
    {
        //create shirt Texture
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);
        selectedShirt.filterMode = FilterMode.Point;

        //Calculate coordinates for shirt pixels
        int y = (shirtStyleNo / shirtSpriteWidth) * shirtTextureHeight;
        int x = (shirtStyleNo % shirtSpriteWidth) * shirtTextureWidth;

        //get shirts pixel
        Color[] shirtPixels = shirtBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);

        //apply selected shirt pixel to texture
        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }

    private void ApplyShirtTextureToBase()
    {
        //Create new shirt base texture
        characterBaseShirtUpdated = new Texture2D(characterBaseTexture.width, characterBaseTexture.height);
        characterBaseShirtUpdated.filterMode = FilterMode.Point;

        //set shirt base texture to transparent
        SetTextureToTransparent(characterBaseShirtUpdated);

        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;

        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        //loop through base texture and apply shirt pixels
        for (int x = 0; x < bodyColumns; x++)
        {
            for (int y = 0; y < bodyRows; y++)
            {
                int pixelX = x * characterSpriteWidth;
                int pixelY = y * characterSpriteHeight;

                if (bodyShirtOffsetArray[x, y] != null)
                {
                    if (bodyShirtOffsetArray[x, y].x == 99 && bodyShirtOffsetArray[x, y].y == 99)// do not populate with shirt
                        continue;
                    pixelX += bodyShirtOffsetArray[x, y].x;
                    pixelY += bodyShirtOffsetArray[x, y].y;
                }
                //swithc on facing direction
                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;

                    case Facing.down:
                        //populate front shirt pixels
                        characterBaseShirtUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                        break;
                    case Facing.up:
                        //populate back shirt pixels
                        characterBaseShirtUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                        break;

                    case Facing.right:
                        //populate right shirt pixels
                        characterBaseShirtUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                        break;

                    default:
                        break;
                }
            }
        }

        //apply shirt texture pixels
        characterBaseShirtUpdated.Apply();
    }

    private void SetTextureToTransparent(Texture2D texture2D)
    {
        //fill texture with transparency
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for (int i = 0; i < fill.Length; i++)
        {
            fill[i] = Color.clear;
        }
        texture2D.SetPixels(fill);
    }

    private void ProcessArms()
    {
        //Get arm pixels to recolor
        Color[] characterPixelsToRecolor = characterBaseTexture.GetPixels(0, 0, 288, characterBaseTexture.height);

        //populate arm color swap list
        PopulateArmColorSwapList();

        //change arm colors
        ChangePixelColors(characterPixelsToRecolor, colorSwapList);

        //Set recolor pixels
        characterBaseCustomized.SetPixels(0, 0, 288, characterBaseTexture.height, characterPixelsToRecolor);

        //Apply texture changes
        characterBaseCustomized.Apply();
    }

    private void PopulateArmColorSwapList()
    {
        //Clear color swap list
        colorSwapList.Clear();
        //Arm replacement colros
        colorSwapList.Add(new colorSwap(armTargetColor1, selectedShirt.GetPixel(0, 7)));
        colorSwapList.Add(new colorSwap(armTargetColor2, selectedShirt.GetPixel(0, 6)));
        colorSwapList.Add(new colorSwap(armTargetColor3, selectedShirt.GetPixel(0, 5)));
    }

   


*/
}
