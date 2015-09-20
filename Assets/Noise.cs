using UnityEngine;

//Adapted from http://catlikecoding.com/unity/tutorials/noise/

public delegate float NoiseFunction(Vector3 point, float frequency);

public enum NoiseType
{
    Value,
    Perlin
}

public static class Noise {

    public static readonly NoiseFunction[] valueMethods =
    {
        Value1D,
        Value2D,
        Value3D
    };

    public static readonly NoiseFunction[] perlinMethods =
    {
        Perlin1D,
        Perlin2D,
        Perlin3D
    };

    public static readonly NoiseFunction[][] noiseMethods =
    {
        valueMethods,
        perlinMethods
    };

    private static int[] hash = {
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,
        151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
        140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
        247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
         57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
         74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
         60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
         65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
        200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
         52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
        207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
        119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
        129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
        218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
         81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
        184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
        222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
    };
    private const int hashMask = 255;
    private static float[] gradients1D = {
        1f, -1f
    };
    private const int gradientsMask1D = 1;
    private static Vector2[] gradients2D = {
        new Vector2(1f, 0f),
        new Vector2(-1f, 0f),
        new Vector2(0f, 1f),
        new Vector2(0f, -1f),
        new Vector2(1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2(1f, -1f).normalized,
        new Vector2(-1f, -1f).normalized
    };
    private const int gradientsMask2D = 7;
    private static Vector3[] gradients3D = {
        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 1f,-1f, 0f),
        new Vector3(-1f,-1f, 0f),
        new Vector3( 1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3( 1f, 0f,-1f),
        new Vector3(-1f, 0f,-1f),
        new Vector3( 0f, 1f, 1f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f, 1f,-1f),
        new Vector3( 0f,-1f,-1f),

        new Vector3( 1f, 1f, 0f),
        new Vector3(-1f, 1f, 0f),
        new Vector3( 0f,-1f, 1f),
        new Vector3( 0f,-1f,-1f)
    };
    private const int gradientsMask3D = 15;

    public static float Value1D(Vector3 point, float frequency)
    {
        point *= frequency;
        int i = Mathf.FloorToInt(point.x);
        float pos = point.x - i;
        i &= hashMask;
        int iNext = i + 1;
        pos = Smooth(pos);
        return Mathf.Lerp(hash[i], hash[iNext], pos) / hashMask;
    }

    public static float Value2D(Vector3 point, float frequency)
    {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        //the gradient position is determined by how far we are from the last integer step
        float posx = point.x - ix0;
        float posy = point.y - iy0;
        //restrict the coordinates to be valid indices in our hash table
        ix0 &= hashMask;
        iy0 &= hashMask;

        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        //hashes for the left and right edges
        int hash0 = hash[ix0];
        int hash1 = hash[ix1];
        //hashes for each corner - naming convention "hash<x><y>"
        int hash00 = hash[hash0 + iy0];
        int hash01 = hash[hash0 + iy1];
        int hash10 = hash[hash1 + iy0];
        int hash11 = hash[hash1 + iy1];

        posx = Smooth(posx);
        posy = Smooth(posy);

        return Mathf.Lerp(
            Mathf.Lerp(hash00, hash10, posx),
            Mathf.Lerp(hash01, hash11, posx),
            posy) / hashMask;
    }

    public static float Value3D(Vector3 point, float frequency)
    {
        point *= frequency;
        int ix = Mathf.FloorToInt(point.x);
        int iy = Mathf.FloorToInt(point.y);
        int iz = Mathf.FloorToInt(point.z);
        //the gradient position is determined by how far we are from the last integer step
        float posx = point.x - ix;
        float posy = point.y - iy;
        float posz = point.z - iz;
        //restrict the coordinates to be valid indices in our hash table
        ix &= hashMask;
        iy &= hashMask;
        iz &= hashMask;

        //hashes for each corner of the current coordinate "box"
        int[,,] hashes = new int[2, 2, 2];
        for (int x = 0; x < 2; x++) {
            int hashX = hash[ix + x];
            for (int y = 0; y < 2; y++) {
                int hashY = hash[hashX + iy + y];
                for (int z = 0; z < 2; z++) {
                    hashes[x, y, z] = hash[hashY + iz + z];
                }
            }
        }

        posx = Smooth(posx);
        posy = Smooth(posy);
        posz = Smooth(posz);

        return Mathf.Lerp(
            Mathf.Lerp(
                Mathf.Lerp(hashes[0, 0, 0], hashes[1, 0, 0], posx),
                Mathf.Lerp(hashes[0, 1, 0], hashes[1, 1, 0], posx),
                posy),
            Mathf.Lerp(
                Mathf.Lerp(hashes[0, 0, 1], hashes[1, 0, 1], posx),
                Mathf.Lerp(hashes[0, 1, 1], hashes[1, 1, 1], posx),
                posy),
            posz) / hashMask;
    }

    public static float Perlin1D(Vector3 point, float frequency)
    {
        point *= frequency;
        int i0 = Mathf.FloorToInt(point.x);
        float pos0 = point.x - i0;
        float pos1 = pos0 - 1f;
        i0 &= hashMask;
        int i1 = i0 + 1;

        float dir0 = gradients1D[hash[i0] & gradientsMask1D];
        float dir1 = gradients1D[hash[i1] & gradientsMask1D];
        float val0 = dir0 * pos0;
        float val1 = dir1 * pos1;

        float pos = Smooth(pos0);
        return Mathf.Lerp(val0, val1, pos) * 2f;
    }

    public static float Perlin2D(Vector3 point, float frequency)
    {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        //the gradient position is determined by how far we are from the last integer step
        float posx0 = point.x - ix0;
        float posy0 = point.y - iy0;
        //gradients are periodic with each unit step, so the next gradient value is at position pos - 1
        float posx1 = posx0 - 1f;
        float posy1 = posy0 - 1f;
        //restrict the coordinates to be valid indices in our hash table
        ix0 &= hashMask;
        iy0 &= hashMask;

        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        //hashes for the left and right edges
        int hash0 = hash[ix0];
        int hash1 = hash[ix1];
        //gradient directions for each corner - naming convention "hash<x><y>"
        Vector2 dir00 = gradients2D[hash[hash0 + iy0] & gradientsMask2D];
        Vector2 dir01 = gradients2D[hash[hash0 + iy1] & gradientsMask2D];
        Vector2 dir10 = gradients2D[hash[hash1 + iy0] & gradientsMask2D];
        Vector2 dir11 = gradients2D[hash[hash1 + iy1] & gradientsMask2D];
        //gradient values for each corner (same naming conventions as the directions)
        float val00 = Vector2.Dot(dir00, new Vector2(posx0, posy0));
        float val01 = Vector2.Dot(dir01, new Vector2(posx0, posy1));
        float val10 = Vector2.Dot(dir10, new Vector2(posx1, posy0));
        float val11 = Vector2.Dot(dir11, new Vector2(posx1, posy1));

        float posx = Smooth(posx0);
        float posy = Smooth(posy0);

        return Mathf.Lerp(
            Mathf.Lerp(val00, val10, posx),
            Mathf.Lerp(val01, val11, posx),
            posy) * Mathf.Sqrt(2);
    }

    public static float Perlin3D(Vector3 point, float frequency)
    {
        point *= frequency;
        int ix0 = Mathf.FloorToInt(point.x);
        int iy0 = Mathf.FloorToInt(point.y);
        int iz0 = Mathf.FloorToInt(point.z);
        //the gradient position is determined by how far we are from the last integer step
        float posx0 = point.x - ix0;
        float posy0 = point.y - iy0;
        float posz0 = point.z - iz0;
        //gradients are periodic with each unit step, so the next gradient value is at position pos - 1
        float posx1 = posx0 - 1f;
        float posy1 = posy0 - 1f;
        float posz1 = posz0 - 1f;
        //restrict the coordinates to be valid indices in our hash table
        ix0 &= hashMask;
        iy0 &= hashMask;
        iz0 &= hashMask;

        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;
        int iz1 = iz0 + 1;

        //The following calculations could, in theory, be done using arrays and loops. However, 3d noise generation is a rather
        //intensive process and the function emperically performs much worse when using loops. Thus, we keep it as a linear
        //sequence of assignments.

        int h0 = hash[ix0];
        int h1 = hash[ix1];
        int h00 = hash[h0 + iy0];
        int h10 = hash[h1 + iy0];
        int h01 = hash[h0 + iy1];
        int h11 = hash[h1 + iy1];
        //gradient directions for each corner of the current coordinate "box" - naming convention "dir<x><y><z>"
        Vector3 dir000 = gradients3D[hash[h00 + iz0] & gradientsMask3D];
        Vector3 dir100 = gradients3D[hash[h10 + iz0] & gradientsMask3D];
        Vector3 dir010 = gradients3D[hash[h01 + iz0] & gradientsMask3D];
        Vector3 dir110 = gradients3D[hash[h11 + iz0] & gradientsMask3D];
        Vector3 dir001 = gradients3D[hash[h00 + iz1] & gradientsMask3D];
        Vector3 dir101 = gradients3D[hash[h10 + iz1] & gradientsMask3D];
        Vector3 dir011 = gradients3D[hash[h01 + iz1] & gradientsMask3D];
        Vector3 dir111 = gradients3D[hash[h11 + iz1] & gradientsMask3D];
        //gradient values for each corner of the current coordinate "box" (same naming convention as above)
        float val000 = Vector3.Dot(dir000, new Vector3(posx0, posy0, posz0));
        float val100 = Vector3.Dot(dir100, new Vector3(posx1, posy0, posz0));
        float val010 = Vector3.Dot(dir010, new Vector3(posx0, posy1, posz0));
        float val110 = Vector3.Dot(dir110, new Vector3(posx1, posy1, posz0));
        float val001 = Vector3.Dot(dir001, new Vector3(posx0, posy0, posz1));
        float val101 = Vector3.Dot(dir101, new Vector3(posx1, posy0, posz1));
        float val011 = Vector3.Dot(dir011, new Vector3(posx0, posy1, posz1));
        float val111 = Vector3.Dot(dir111, new Vector3(posx1, posy1, posz1));

        float posx = Smooth(posx0);
        float posy = Smooth(posy0);
        float posz = Smooth(posz0);

        return Mathf.Lerp(
            Mathf.Lerp(
                Mathf.Lerp(val000, val100, posx),
                Mathf.Lerp(val010, val110, posx),
                posy),
            Mathf.Lerp(
                Mathf.Lerp(val001, val101, posx),
                Mathf.Lerp(val011, val111, posx),
                posy),
            posz);
    }

    /// <summary>
    /// Performs fractal noise accumulation in order to create more natural-looking, detailed noise
    /// </summary>
    public static float Sum(NoiseFunction function, Vector3 point, float frequency, int octaves, float lacunarity, float gain)
    {
        float sum = function(point, frequency);
        float amplitude = 1f;
        float range = 1f;
        for (int i = 1; i < octaves; i++)
        {
            frequency *= lacunarity;
            amplitude *= gain;
            range += amplitude;
            sum += function(point, frequency) * amplitude;
        }
        return sum / range;
    }

    /// <summary>
    /// Applies the Perlin smoothing function, 6t^5 -15t^4 + 10t^3
    /// </summary>
    private static float Smooth(float t)
    {
        //Since there is no atomic exponentiation, rewrite the function to use only atomic operations
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }
}
