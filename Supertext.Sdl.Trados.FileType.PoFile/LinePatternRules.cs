using System;

namespace Supertext.Sdl.Trados.FileType.PoFile
{
    public static class LinePatternRules
    {
        public static LinePattern GetPoFileStartingLinePattern()
        {
            var start = new LinePattern(String.Empty);
            var msgid = new LinePattern(@"msgid\s+"".*""");
            var msgstr = new LinePattern(@"msgstr\s+"".*""");
            var text = new LinePattern("\"");
            var comment = new LinePattern("#");

            start
                .MustBeFollowedBy(msgid)
                .CanBeFollowedBy(comment);

            msgid
                .MustBeFollowedBy(msgstr)
                .CanBeFollowedBy(text);

            msgstr
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgid);

            text
                .CanBeFollowedBy(text)
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgstr)
                .CanBeFollowedBy(msgid);

            comment
                .CanBeFollowedBy(comment)
                .CanBeFollowedBy(msgid);

            return start;
        }
    }
}
