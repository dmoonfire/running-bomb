8-April-2006

=Colombo Hydrogen Drumkit and free sound samples=
Version: 1.0
(C) 2006 by Marcos Guglielmetti
Licence: GNU GPL v2 or later, see COPYING for details

=Example=

http://www.pc-musica.com.ar/musix/ogg/README-colombo-example1.txt
http://www.pc-musica.com.ar/musix/ogg/colombo-example1-by-mgg.ogg


=Main Index=

1) Licence
2) Installation
3) Recording details



=1) Licence=

These sound samples and the hydrogen drumkit are free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.

These sound samples and the hydrogen drumkit are distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

As a special exception, if you create a composition which uses these sound samples and/or the hydrogen drumkit, and mix these sound samples and/or the hydrogen drumkit or unaltered portions of these sound samples and/or the hydrogen drumkit into the composition, these sound samples and/or the hydrogen drumkit do not by themselves cause the resulting composition to be covered by the GNU General Public License. 

This exception does not however invalidate any other reasons why the document might be covered by the GNU General Public License. If you modify these sound samples and/or the hydrogen drumkit, you may extend this exception to your version of the sound samples and/or the hydrogen drumkit, but you are not obligated to do so. If you do not wish to do so, delete this exception statement from your version.

You should have received a copy of the GNU General Public License along with these sound samples and the hydrogen drumkit; if not, write to the

Free Software Foundation, Inc.
59 Temple Place - Suite 330
Boston, MA 02111-1307, USA.

=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-


=2) Installation=

To use the Colombo hydrogen's drumkit:

You must make a dir into /usr/share/hydrogen/data/drumkits/ (or into your hydrogen's installation path)

As instance:
(Console command)
 mkdir /usr/share/hydrogen/data/drumkits/Colombo-Acustic-Drumkit

Then copy all the files to the new directory.

Console commands:
 cd /(your path to the samples & drumkit)

 cp *.* /usr/share/hydrogen/data/drumkits/Colombo-Acustic-Drumkit

Then you can use it with Hydrogen, just open this file from Hydrogen
/usr/share/hydrogen/data/drumkits/Colombo-Acustic-Drumkit/Colombo-Acustic-Drumkit-v1.h2song

or type

(Console command)
hydrogen /usr/share/hydrogen/data/drumkits/Colombo-Acustic-Drumkit/Colombo-Acustic-Drumkit-v1.h2song

=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

=3) Recording details=

(7-4-2006)
Yesterday I was recording my acustic drum: it's not the best drum in the world, but you can get very nice sounds from it... it is a

* Colombo Percussion, handmade in Argentina circa 1970 to 1980

Similar to these models:

http://www.deremate.com.ar/accdb/viewItem.asp?IDI=11842530
http://www.deremate.com.ar/accdb/viewItem.asp?IDI=10025439
http://www.deremate.com.ar/accdb/viewItem.asp?IDI=11045984

=Aesthetics=

I was not trying to get a "dry" sound: that's why added reverb to almost all of them.

The hihat and ride sounds have no extra reverb.

There is only one snare sample without extra reverb:
snare-opaque-2mics_normal-shot1.flac

(if you like it, I could make a new collection of dry samples)

I will make a "more wet" (adding some extra reverb) collection in the future, and maybe with another EQ/compression settings: it could be useful when you are making a Soundfont (.sf2) patch and you need your MIDI music to sound as it was already mastered.

By using the Hydrogen Drumkits, you can edit every single Instrument adding LADSPA plugins, so, maybe it's not a big deal to add some extra reverb to the sound samples files for now.

=Drums's measurement & details= 

Wooden toms, snare and bassdrum

Toms: 12X8 13X9 16X16 (hi, mid, low-foor)
Bassdrum: 21X14
Snare "piccolo" 14X4
Aquarian patch set. (¿parche se dice patch?)

Crash 16'' Sabian "B8 Pro, Thin Crash"
Crash 20'' Paiste 1000 "Power Crash 20''", made in Switzerland
Hihat Paiste 1000 14'' "Rude", made in Switzerland


=Room & equipment=

Recorded in my "acustic ready" room:

Mixer console: Phonic MU 802 (I have acces to better consoles such as Makie and Peavey, but this Phonic it's not so bad: it has Phantom Power too)
Mics: 6 Shure SM58 (I could borrow a Shure SM81 condenser Mic from a friend for the next time!)

=PC=

AMD Duron 1800, 128 RAM memory, 2 HD 30GB 7200RPM, SB Live!


=Software=

Operating System: Musix GNU+Linux 
Apps: Ardour, LADSPA, Rezound, Hydrogen

The sound samples were processed with Ardour & chained LADSPA plugins:

Multiband EQ
Gate
Fast Lookahead limiter
Caps: Versatile verb 2x2

Then, the samples were trimed with Rezound and saved as flac (Free Lossless Audio Codec) files.


=Future work=

The complete collection of .flac samples weight's it is about 12mb without the "shine" snares (that I did not finished yet, as you can listen).


=Mail me!=

Suggestions to:
marcospcmusica@gmail.com

If you make some music with this sounds, I will like to listen to it!

=File list=

 ls -lh
total 12M

6,9K  README-Colombo-Drumkit.txt
74K 2006-04-07 01:52 Colombo-Acustic-Drumkit-v1.h2song
15K COPYING

  57K  bassdrum-4mics-br-stereo-1.flac
  40K  bassdrum-4mics-br-stereo-medium1.flac
  39K  bassdrum-4mics-br-stereo-normal1.flac
  44K  bassdrum-4mics-br-stereo-normal2.flac
  39K  bassdrum-4mics-br-stereo-normal3.flac
  39K  bassdrum-4mics-br-stereo-soft1.flac
  39K  bassdrum-4mics-br-stereo-soft2.flac
  36K  bassdrum-4mics-br-stereo-soft3.flac
  39K  bassdrum-4mics-br-stereo-soft4.flac
 295K  crash16i__crash1.flac
 348K  crash16i__crash2.flac
 317K  crash16i__crash3.flac
 326K  crash16i__ride1.flac
 215K  crash16i__ride2.flac
 294K  crash16i__ride3.flac
 182K  crash16i__ride4.flac
 254K  crash16i__ride5.flac
 142K  crash16i__ride_cup1.flac
 211K  crash16i__ride_cup2.flac
 279K  crash16i__ride_cup3.flac
 187K  crash16i__ride_cup4.flac
 281K  crash20i__crash.flac
 230K  crash20i__ride1.flac
 291K  crash20i__ride2.flac
 474K  crash20i__ride3.flac
 305K  crash20i__ride4.flac
 302K  crash20i__ride5.flac
 265K  crash20i__ride-cup1.flac
 153K  crash20i__ride-cup2.flac
 224K  crash20i__ride-cup3.flac
 192K  crash20i__ride-cup4.flac
 249K  crash20i__ride-cup5.flac
  44K  hihat-closed-1.flac
  38K  hihat-closed-2.flac
  42K  hihat-closed-3.flac
  46K  hihat-closed-4.flac
  37K  hihat-closed-5.flac
  42K  hihat-closed-6.flac
  51K  hihat-closed-7.flac
 136K  hihat-open-1.flac
 119K  hihat-open-2.flac
 274K  hihat-open-3.flac
 280K  hihat-open-4.flac
 178K  hihat-open-5.flac
 103K  hihat-open-6.flac
  66K  hihat-open-and-close1.flac
  73K  hihat-open-and-close2.flac
 116K  hihat-open-and-close3.flac
  71K  hihat-open-and-close4.flac
  72K  hihat-semi-closed-1.flac
  78K  hihat-semi-open-1.flac
  73K  hihat-semi-open-2.flac
   71K  snare-opaque-2mics_normal-shot1.flac
  98K  snare-opaque-2mics_rimshot1.flac
  84K  snare-opaque-normal-mic-1.flac
  80K  snare-opaque-normal-mic-2.flac
  74K  snare-opaque-normal-mic-double_shot1.flac
  62K  snare-opaque-normal-mic-normal_shot2.flac
  63K  snare-opaque-normal-mic-normal_shot3.flac
  80K  snare-opaque-normal-mic-rimshot1.flac
  77K  snare-opaque-normal-mic-rimshot2.flac
  79K  snare-opaque-normal-mic-rimshot3.flac
  85K  snare-opaque-normal-mic-rimshot4.flac
  75K  snare-opaque-normal-mic-rimshot5.flac
  75K  snare-opaque-normal-mic-rimshot6.flac
  77K  snare-opaque-normal-mic-rimshot7.flac
  81K  snare-opaque-normal-mic-rimshot8.flac
  76K  snare-opaque-normal-mic-rimshot9.flac
  45K  stick-rim-1.flac
  39K  stick-rim-2.flac
  34K  stick-rim-3.flac
  42K  stick-rim-4.flac
  61K  stick-rim-5.flac
  58K  stick-rim-6.flac
  54K  stick-rim-7.flac
  57K  stick-rim-8.flac
  62K  stick-rim-9.flac
  80K  tom-hi-1.flac
  87K  tom-hi-2.flac
  88K  tom-hi-3.flac
  81K  tom-hi-4.flac
  81K  tom-hi-double-1.flac
  81K  tom-hi-double-2.flac
  72K  tom-low-1.flac
  76K  tom-low-2.flac
  78K  tom-low-double-1.flac
  70K  tom-low-double-2.flac
  64K  tom-mid-1.flac
  76K  tom-mid-2.flac
  82K  tom-mid-3.flac
  73K  tom-mid-double-1.flac
  75K  tom-mid-double-2.flac
