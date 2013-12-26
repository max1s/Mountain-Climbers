using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PerlinNoise
{
	
	public static float[,] GenerateNoise(int width, int height)
    {            
        float[,] noise = new float[width,height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                noise[i,j] = Random.value;
            }
        }
        return noise;
    }

 	public static float[,] Smooth(float[,] baseNoise, int octave, int width, int height)
    {
        float[,] smoothNoise = new float[width, height];

        int smoothPeriod = 8 << octave;
        float smoothFrequency = 1.0f / smoothPeriod;

        for (int i = 0; i < width; i++)
        {
            int firstHorizontalSample = (i / smoothPeriod) * smoothPeriod;
            int secondHorizontalSample = (firstHorizontalSample + smoothPeriod) % width; 
            float horizontalBlend = (i - firstHorizontalSample) * smoothFrequency;

            for (int j = 0; j < height; j++)
            {
                int firstVerticalSample = (j / smoothPeriod) * smoothPeriod;
                int secondVerticalSample = (firstVerticalSample + smoothPeriod) % height;
                float VerticalBlend = (j - firstVerticalSample) * smoothFrequency;

                Vector2 interpolateCorners = new Vector2(Mathf.Lerp(baseNoise[firstHorizontalSample,firstVerticalSample], baseNoise[secondHorizontalSample,firstVerticalSample], horizontalBlend),
													     Mathf.Lerp(baseNoise[firstHorizontalSample,secondVerticalSample], baseNoise[secondHorizontalSample,secondVerticalSample], horizontalBlend));

                smoothNoise[i,j] = Mathf.Lerp(interpolateCorners.x, interpolateCorners.y, VerticalBlend);                    
            }
        }
        
        return smoothNoise;
    }
	
	public static float[,] Blend(float[,] baseNoise, int octaveCount,int width,int height, float[] promenance)
    {
        List<float[,]> smoothNoise = new List<float[,]>();
		
        for (int i = 0; i < octaveCount; i++)
        {
			smoothNoise.Add( Smooth(baseNoise, i, width, height));
        }

        float[,] perlinNoise = new float[width, height];

        float totalAmplitude = 0.0f;
        //blend noise together
        for (int octave = octaveCount - 1; octave >= 0; octave--)
        {
			float tempPromenance = promenance[octaveCount - 1 - octave];
            totalAmplitude += tempPromenance;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i,j] += smoothNoise[octave][i,j] * tempPromenance;
                }
            }
        }
        //normalisation
        for (int i = 0; i < width; i++){for (int j = 0; j < height; j++){perlinNoise[i,j] /= totalAmplitude;}}        
		
        return perlinNoise;
    }
	
 	public static float[,] Blend(float[,] baseNoise, int octaveCount,int width,int height, float persistance, float amplitude)
    {
    	List<float[,]> smoothNoise = new List<float[,]>();
		
		for (int i = 0; i < octaveCount; i++)
		{
				smoothNoise.Add( Smooth(baseNoise, i, width, height));
		}

        float[,] perlinNoise = new float[width, height];

        float totalAmplitude = 0.0f;
        //blend noise together
        for (int octave = octaveCount - 1; octave >= 0; octave--)
        {
            amplitude *= persistance;
            totalAmplitude += amplitude;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i,j] += smoothNoise[octave][i,j] * amplitude;
                }
            }
        }
        //normalisation
        for (int i = 0; i < width; i++){for (int j = 0; j < height; j++){perlinNoise[i,j] /= totalAmplitude;}}        
		
        return perlinNoise;
    }

        public static float[,] Blend(int width, int height, int octaveCount, float persistance, float amplitude)
        {
            return Blend( GenerateNoise(width, height), octaveCount, width, height, persistance, amplitude );
        }
	
		public static float[,] Blend(int width, int height, int octaveCount, float[] prominence)
        {
            return Blend( GenerateNoise(width, height), octaveCount, width, height, prominence );
        }
}
