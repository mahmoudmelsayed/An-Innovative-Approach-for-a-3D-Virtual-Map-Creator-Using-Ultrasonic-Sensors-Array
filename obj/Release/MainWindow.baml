   M S B A M L   `   `   `  ���� \  XHelixToolkit.Wpf, Version=2015.1.715.0, Culture=neutral, PublicKeyToken=52aa3500039caf0dM8clr-namespace:HelixToolkit.Wpf;assembly=HelixToolkit.WpfHelixToolkit.Wpf   
Mapper.Wpf   Mapper.Wpf.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Jhelix8clr-namespace:HelixToolkit.Wpf;assembly=HelixToolkit.Wpf     -     � Title$  	3D Mapper��$	��750��$
��1000��.���  � RowDefinitions �� $��* ��� $
��Auto �.+�� $��0��$��0�� � ColumnDefinitions �� $��* ��� $
��Auto �.+�&    HelixToolkit.Wpf.HelixViewport3D  -     Name$ Viewport��   ZoomExtentsWhenLoaded .    ShowCoordinateSystem .    Children. $   HelixToolkit.Wpf.DefaultLights  $   HelixToolkit.Wpf.LinesVisual3D  -   
MapPlotter��*   $HelixToolkit.Wpf.ScreenSpaceVisual3D   Color$	 Red��   	Thickness$ 2Q�	   Points	 7� 
 7� X$
 2Q� 7� Y$ 0Q� 7� Z$ 1Q�7� $
 1Q�$ 1Q�$ 1Q�7� $
 1Q�$ 1Q�$ 1Q�7� $
 0Q�$ 2Q�$ 0Q�7� $
 3Q�$ 1Q�$ 1Q�7� $
 4Q�$ 3Q�$ 0Q�(   "HelixToolkit.Wpf.GridLinesVisual3D     Width$	 200Q�   Length$	 200Q�   MinorDistance$ 1Q�   MajorDistance$ 10Q�   	Thickness$
 0.01Q�(   "HelixToolkit.Wpf.FileModelVisual3D  -   FileModelVisual3D��� $��1��$	��400��.+�� $��12,6,0,0q� �� $
��Auto ��� .+��� $��0,0,0,4q�$
��logs��$��Segoe UI)�$��20(�$��Light"��� -    ��Name$ LogScrollViewer��$��1��$
��Auto=�.���� -    ��Name$ 
LogTextBox��$��Consolas)�$��12(� �� TextWrapping$
 Wrap=� �� 
IsReadOnly . � $��1��$	��200�� �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��20,12q�$	��150��.+��� $��0,0,0,4q�$��controls��$��Segoe UI)�$��20(�$��Light"��� -    ��Name$ StartButton��$	��0,2q�$��Start��{�.  �� -   $ 
StopButton��$	��0,2q�$
��Stop��{�.  �� -	   $ DisconnectButton��$��0,2,0,0q�$��
Disconnect��{�.  �� -
   $ ResetViewButton��$��0,20,0,0q�$��	Reset Map���� $��1��$��20,12q�$	��150��.+��� $��0,0,0,4q�$
��cars��$��Segoe UI)�$��20(�$��Light"��� -    ��Name$ CarNameTextBlock��$��	0,-10,0,4q�$��Segoe UI)�$��20(�$��SemiBold"��� $��2��$��20,12q�$	��400��.+��� $��0,0,0,4q�$��settings��$��Segoe UI)�$��20(�$��Light"��  �� �� $��5 ��� $
��Auto �.+��� .+��� $��0,4,0,4q�$��MOTOR SPEED��$��Segoe UI)�$��12(�$��SemiBold"��� -    ��Name$ SpeedSlider�� �� IsSnapToTickEnabled .  � Minimum$ 80Q� � Maximum$	 250Q� �� TickFrequency$ 2Q� �� TickPlacement$ BottomRight=�{�.  �� $��2��$��0,0,0,4q� �� TextAlignment$ Right=�$��40��$��Segoe UI)�$��20(�$��Light"����� *Value+ �� ElementName$ SpeedSlider��  �� StringFormat$  0���  �� �� $��5 ��� $
��Auto �.+��� .+��� $��0,4,0,4q�$+��%MIN DISTANCE BETWEEN CAR AND OBSTACLE��$��Segoe UI)�$��12(�$��SemiBold"��� -   $ MinDistanceSlider�� . $ 10Q�$ 50Q�$ 1Q�$ BottomRight=�{�.  �� $��2��$��0,0,0,4q�$ Right=�$��40��$��Segoe UI)�$��20(�$��Light"����� *Value+$ MinDistanceSlider��$  0���� -   $ 
SaveButton��$��0,14,0,0q�$��Save Settings��{�.  