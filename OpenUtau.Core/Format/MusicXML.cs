using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using OpenUtau.Core;
using OpenUtau.Core.Ustx;
using Serilog;

namespace OpenUtau.Core.Format {
    public static class MusicXML {
        static public UProject LoadProject(string file) {
            UProject uproject = new UProject();
            Ustx.AddDefaultExpressions(uproject);
            uproject.tracks = new List<UTrack>();

            var score = readXMLScore(file);
            List<string> lyrics = new List<string>();
            foreach(var measure in score.Part[0].Measure) {
                foreach(var note in measure.Note) {
                    if (note.Lyric.Count > 0) {
                        lyrics.Add(note.Lyric[0].Text[0].Value);
                        var pitch = note.Pitch.Step.ToString()+note.Pitch.Octave.ToString();
                        var tone = MusicMath.NameToTone(pitch).ToString();
                        lyrics.Add(pitch);
                        lyrics.Add(tone);
                        lyrics.Add(note.Duration.ToString());
                    }
                }
            }
            Log.Information($"Lyrics: {string.Join(" ", lyrics)}");

            return uproject;
        }
        static public List<UVoicePart> Load(string file, UProject project) {
            List<UVoicePart> resultParts = new List<UVoicePart>();
            return resultParts;
        }
        static public MusicXMLSchema.ScorePartwise readXMLScore(string xmlFile) {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.MaxCharactersFromEntities = 1024;

            using (FileStream stream = new FileStream(xmlFile, FileMode.Open)) {
                XmlReader reader = XmlReader.Create(stream, settings);
                XmlSerializer s = new XmlSerializer(typeof(MusicXMLSchema.ScorePartwise));

                var score = s.Deserialize(reader) as MusicXMLSchema.ScorePartwise;
                return score;
            }
        }
    }
}
