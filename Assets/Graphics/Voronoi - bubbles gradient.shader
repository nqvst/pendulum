//https://www.ronja-tutorials.com/2018/09/29/voronoi-noise.html

Shader "Custom/Mosaic"{
	Properties {
		_CellSize ("Cell Size", Range(0, 2)) = 2
		_TintColor ("Tint", Color) = (0,0,0,1)
		_TimeScale ("Scrolling Speed", Range(0, 2)) = 1
		_BorderThickness("Border Thickness", Range(0, 2)) = 1
	}
	SubShader {
		
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off 
		
		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		#include "random.inc"
		// random functions by https://gist.github.com/h3r/3a92295517b2bee8a82c1de1456431dc
		float _CellSize;
		float3 _TintColor;
		float _TimeScale;

		struct Input {
			float3 worldPos;
		};

		float3 voronoiNoise(float3 value){
			float3 baseCell = floor(value);
			float minDistToCell = 10;
			float3 toClosestCell;
			float3 closestCell;
			[unroll]
			for(int x1=-1; x1<=1; x1++){
				[unroll]
				for(int y1=-1; y1<=1; y1++){
					[unroll]
					for(int z1=-1; z1<=1; z1++){
						float3 cell = baseCell + float3(x1, y1, z1);
						float3 cellPosition = cell + rand3dTo3d(cell);
						float3 toCell = cellPosition - value;
						float distToCell = length(toCell);
						if (distToCell < minDistToCell){
							minDistToCell = distToCell;
							closestCell = cell;
							toClosestCell = toCell;
						}
					}
				}
			}

			float minEdgeDistance = 10;
			[unroll]
			for(int x2=-1; x2<=1; x2++){
				[unroll]
				for(int y2=-1; y2<=1; y2++){
					[unroll]
					for(int z2=-1; z2<=1; z2++){
						float3 cell = baseCell + float3(x2, y2, z2);
						float3 cellPosition = cell + rand3dTo3d(cell);
						float3 toCell = cellPosition - value;

						float3 diffToClosestCell = abs(closestCell - cell);
						bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
						if(!isClosestCell){
							float3 toCenter = (toClosestCell + toCell) * 0.5;
							float3 cellDifference = normalize(toCell - toClosestCell);
							float edgeDistance = dot(toCenter, cellDifference);
							minEdgeDistance = min(minEdgeDistance, edgeDistance);
						}
					}
				}
			}

			float random = rand3dTo1d(closestCell);
			return float3(minDistToCell, random, minEdgeDistance);
		}

		void surf (Input i, inout SurfaceOutputStandard o) {
			float3 value = i.worldPos.xyz / _CellSize;
			value.z += _Time.y * _TimeScale;
			float3 noise = voronoiNoise(value);
			
			float3 cellColor = rand1dTo3d(noise.y); 
			float valueChange = fwidth(value.z) * 0.5;
			float isBorder = 1 - smoothstep(0.05 - valueChange, 0.05 + valueChange, noise.z);
			float3 color = lerp(cellColor, _TintColor, 0);
			
			o.Albedo = noise.x * color;
		}
		ENDCG
	}
	FallBack "Standard"
}