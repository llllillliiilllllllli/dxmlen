using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxMLEngine.Features.OxfordDictionary
{
    internal class Endpoint
    {
        private const string API_ID = @"d6677759";
        private const string API_KEY = @"ea97976db4e3d50c7d9f06493b490e3e";

        private const string EP_BASE = "https://od-api.oxforddictionaries.com/api/v2";
        private const string EP_ENTRIES = "https://od-api.oxforddictionaries.com/api/v2/entries/{source_lang}/{word_id}";
        private const string EP_LEMMAS = "https://od-api.oxforddictionaries.com/api/v2/lemmas/{source_lang}/{word_id}";
        private const string EP_SEARCH_TRANSLATION = "https://od-api.oxforddictionaries.com/api/v2/search/translations/{source_lang_search}/{target_lang_search}";
        private const string EP_SEARCH = "https://od-api.oxforddictionaries.com/api/v2/search/translations/{source_lang}";
        private const string EP_SEARCH_THESAURUS = "https://od-api.oxforddictionaries.com/api/v2/search/thesaurus/{source_lang}";
        private const string EP_TRANSLATION = "https://od-api.oxforddictionaries.com/api/v2/translations/{source_lang_translate}/{target_lang_translate}/{word_id}";
        private const string EP_THESAURUS = "https://od-api.oxforddictionaries.com/api/v2/translations/api/v2/thesaurus/{lang}/{word_id}";
        private const string EP_SENTENCES = "https://od-api.oxforddictionaries.com/api/v2/api/v2/sentences/{source_lang}/{word_id}";
        private const string EP_UTILITIES = "...";
        private const string EP_WORDS = "https://od-api.oxforddictionaries.com/api/v2/words/{source_lang}";
        private const string EP_INFLECTIONS = "https://od-api.oxforddictionaries.com/api/v2/inflections/{source_lang}/{word_id}";
    }
}
