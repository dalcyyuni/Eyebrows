   M S B A M L   `   `   `  ����L  HKinect2FaceHD_NET, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Kinect2FaceHD_NETKinect2FaceHD_NET  5      $    Kinect2FaceHD_NET.UserControl2   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:Kinect2FaceHD_NET     5      .��5	      �   � ColumnDefinitions  5
   
   �� 5      $	��300 �6   �� 5      $	��110 �6   �� 5      $	��300 �6    � RowDefinitions 5   
   �� 5      $��54 �6   �� 5      $��45 �6   �� 5      $	��132 �6   �� 5      $��58 �6   �� 5      $��22 �6   �� 5      $��88 �6   .+�5   
   �� $��0��6   $��0��6!   $��3��61   $
��Left=�6E   $
��Wrap=�6�   ��6`   �� *FaceVerificationDescription+6�   � 5   
   $��0��6   $��2��6   $��4��6,   .+�5      ��  K� ZIndex$	 999��6j   ��6   �� *LeftResultCollection+ �� ElementName$ FaceVerificationPane����5      �� 5      .R�5      �� 
������ $��Center=�6:   $��Center=�6W   ��6r   �� *MaxImageSize+$ FaceVerificationPane����6�   �� *MaxImageSize+$ FaceVerificationPane��5      ��5      �� 5        �� 
TargetType" ��6   .�5!      ��  �� Property �@��Left �� Value 68   �� *Left+�� 5"       �@��Top 67   �� *Top+5#      ��5%      �� 5&      .R�5'      �� ���	#FF26B8F4$��1��6i   ��6$   �� *Width+��6<   �� *Height+6i   5(      5*      �� 5+      -   5         ��Name$ LeftImageDisplay��5+      $��Center=�6�   $��Center=�6�   $��0,0,0,0q�6�   $7��1/FaceAPI-WPF-Samples;component/Assets/default.jpg��6�   ��6.   �� *MaxImageSize+$ FaceVerificationPane����6p   �� *MaxImageSize+$ FaceVerificationPane��6�   5,      �� 5-   
   -   5        $��0��5-      $��1��6!   $��Choose Image��6.   $��Center=�6E   $	��Top=�6b   $	��300��6{   $��35��6�   ���#2196f3
�������
������ $��
0, 0, 0, 0q�6�   6�   �� 5.   
   -   5        $��1��5.      $��3��6!   $��Verify��6.   $��90��6?   $��35��6J   ���#2196f3
�������
������ 6�   �� 5/   
   -   5        $��rs��5/   �   $��1��6   $��4��6$   $��10,0,0,0q�61   $
��Left=�6C   $	��Top=�6^   $��Results:��6v   $
��Wrap=�6�   6�   �� 50   
   $��1��6   $��5��6$   $��10,0,0,0q�61   $
��Left=�6C   $	��Top=�6^   $
��Wrap=�6�   ��6v   �� *VerifyResult+$ FaceVerificationPane��6�   � 52   
   $��2��6   $��2��6   $��4��6,   .+�53      �� $	 999��6k   ��6   �� *RightResultCollection+$ FaceVerificationPane����54      �� 55      .R�56      �� 
������ $��Center=�6:   $��Center=�6W   ��6r   �� *MaxImageSize+$ FaceVerificationPane����6�   �� *MaxImageSize+$ FaceVerificationPane��57      ��59      �� 5:      " ��6   .�5;      ��  �@��Left 68   �� *Left+�� 5<       �@��Top 67   �� *Top+5=      ��5?      �� 5@      .R�5A      �� ���	#FF26B8F4$��1��6i   ��6$   �� *Width+��6<   �� *Height+6i   5B      5D      �� 5E      -   5        $ RightImageDisplay��5E   $   $��2��6   $��Center=�6�   $��Center=�6�   $��0,0,0,0q�6�   ��6?   �� *MaxImageSize+$ FaceVerificationPane����6�   �� *MaxImageSize+$ FaceVerificationPane��6�   5F      �� 5G   
   -   5        $��2��5G      $��1��6!   $��Choose Image��6.   $��Center=�6E   $	��Top=�6b   $	��300��6z   $��35��6�   ���#2196f3
�������
������ $��
0, 0, 0, 0q�6�   6�   5H      5I      