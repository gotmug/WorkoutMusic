   M S B A M L   `   `   `  ����G  CWorkoutMusic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WorkoutMusicWorkoutMusic  5   	       WorkoutMusic.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ,localclr-namespace:WorkoutMusic     5   	   -   5          � Title$  
MainWindow��5   	   $	��600��6   $	��800��6)   .��5      �  � ColumnDefinitions 5	   
   �� 5
      $
��.33* �6   �� 5       � RowDefinitions 5   
   �� 5      �� 5      $
��.05* �6   .+�5   
   �� $��1��6   $��2��6!   .@�5      �� ��6   �� *SelectedItemPath+5      R� 5   
   -   5         R�Name$ foldersItem��5      $
��Auto��6^   ���	#FFFFFFFF���	#FFFFFFFF���	#FFFFFFFF � 	Resources 5       �� 
TargetType �� Property �� Value �� Orientation	 �� RelativeSource
 �� Mode �� AncestorType �� Path)   #WorkoutMusic.HeaderToImageConverter   Instance �� 	Converter��%d  'Q�       �� 5      " Q�6   .�5      ��  � �� 5      �� 5      .R�5   "   �� $ 
Horizontal=�6-   .+�5   &   �� $	��img��6,   $��20��68   $��20��6C   $
��Fill=�6O   ��5   (   �� 	 �� $
 FindAncestor=�" Q�$ Header�# Z �� 5#   &   $	��5,0q�6A   ��60   �� 6A   5$   #   5%      5'      5(      5)      5*      � 5,   
   $��1��6    5-      �� 5.      $
��.10* �6    �� 5/      .+�51      c� .@�52      �� ��6   �� *PlayButtonTitle+��6=   �� *PlayCommand+53      &   System.Windows.Controls.DataGrid  54      -   5        $��1��54   M   ��6Z   �� *Songs+ �� SelectedItem 6x   �� *SelectedSong+��55      �� 56      )  #System.Windows.Controls.DataGridRow"  6   .�57      -   5        58      5:      5<      5>      5?      