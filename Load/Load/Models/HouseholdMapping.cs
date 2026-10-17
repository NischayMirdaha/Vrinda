namespace Load.Models;

public static class HouseholdMapping
{
    public static readonly Dictionary<string, ColumnMap> Map = new()
    {
        // ═══════════════════════════════════════════════════
        // HOUSEHOLD — Session + ODK metadata
        // ═══════════════════════════════════════════════════
        ["start"] = new() { Table = "household", Column = "survey_start", Type = ColType.Text },
        ["end"] = new() { Table = "household", Column = "survey_end", Type = ColType.Text },
        ["deviceid"] = new() { Table = "household", Column = "device_id", Type = ColType.Text },
        ["phonenumber"] = new() { Table = "household", Column = "phone_number", Type = ColType.Text },
        ["_id"] = new() { Table = "household", Column = "odk_id", Type = ColType.BigInt },
        ["_uuid"] = new() { Table = "household", Column = "uuid", Type = ColType.Text },
        ["_submission_time"] = new() { Table = "household", Column = "submission_time", Type = ColType.Text },
        ["_validation_status"] = new() { Table = "household", Column = "validation_status", Type = ColType.Text },
        ["_notes"] = new() { Table = "household", Column = "notes", Type = ColType.Text },
        ["_status"] = new() { Table = "household", Column = "status", Type = ColType.Text },
        ["_submitted_by"] = new() { Table = "household", Column = "submitted_by", Type = ColType.Text },
        ["__version__"] = new() { Table = "household", Column = "version", Type = ColType.Text },
        ["_tags"] = new() { Table = "household", Column = "tags", Type = ColType.Text },
        ["_index"] = new() { Table = "household", Column = "odk_index", Type = ColType.Integer },

        // Consent (ignore)
        ["नमस्कार ! म .............. यस वृन्दावन नगरपालिकाको वस्तुस्थिति विवरण तयारीका लागि जानकारी सङ्कलन गर्न उपस्थित भएको छु। यसमा नेपाल सरकारले तयार गरेको गाउँ तथा नगर वस्तुस्थिति विवरण तयारी कार्यविधि २०७८ मा आधारित भई वृन्दावन नगरपालिकाले आफ्नो नगर वस्तुस्थिति विवरण (Municipality Profile) तयार गर्न आवश्यक पर्ने प्रश्\u200Dनावलीहरु रहेकाछन्। तपाईले दिनु भएको जानकारी सूचनाको हक अन्तर्गत गोप्य राखिने छ । सत्यतथ्य जानकारी उपलब्ध गराइ सहयोग गरिदिनु होला ।"] = new() { Table = "household", Column = "consent_text", Type = ColType.Ignore },

        // ═══════════════════════════════════════════════════
        // Section 1: Municipality
        // ═══════════════════════════════════════════════════
        ["१.१ वृन्दावन नगरपालिका"] = new() { Table = "household", Column = "municipality", Type = ColType.Text },
        ["१.२  वडा  नम्बर"] = new() { Table = "household", Column = "ward_no", Type = ColType.Integer },
        ["१.३ फर्म भर्ने गणकको नाम"] = new() { Table = "household", Column = "enumerator_name", Type = ColType.Text },

        // ═══════════════════════════════════════════════════
        // Section 2: House location
        // ═══════════════════════════════════════════════════
        ["२.१ घरको क्रम संख्या (घर नं.)"] = new() { Table = "household", Column = "house_serial_no", Type = ColType.Double },
        ["२.२ वस्तीको नाम"] = new() { Table = "household", Column = "settlement_name", Type = ColType.Text },
        ["२.३ मार्गको नाम"] = new() { Table = "household", Column = "road_name", Type = ColType.Text },
        ["२.४ घर पुग्ने बाटो/सडकको प्रकार"] = new() { Table = "household", Column = "road_type", Type = ColType.Text },
        ["२.५ नगरपालिका केन्द्र देखि यस टोल सम्मको दूरी"] = new() { Table = "household", Column = "distance_from_center", Type = ColType.Text },

        // ═══════════════════════════════════════════════════
        // Section 3: Respondent
        // ═══════════════════════════════════════════════════
        ["३.१ उत्तरदाताकाे नाम  थर"] = new() { Table = "household", Column = "respondent_name", Type = ColType.Text },
        ["३.२ ${Respondent}  को फोन संम्पर्क नम्बर कति हाे ?"] = new() { Table = "household", Column = "respondent_phone", Type = ColType.Text },
        ["३.३ ${Respondent}  उत्तरदाता र घरमुली बीचको नाता के हो ?"] = new() { Table = "household", Column = "respondent_relation", Type = ColType.Text },
        ["अन्य को हो खुलाउनु होला ।"] = new() { Table = "household", Column = "respondent_relation_other", Type = ColType.Text },
        ["३.४ तपाईको परिवार विगत पाँच बर्षमा बसाई सरेर आएको हो ?"] = new() { Table = "household", Column = "migrated_last_5years", Type = ColType.Text },
        ["३.४.१ कहाँबाट बसाइ सर्नु भएको हो ?"] = new() { Table = "household", Column = "migrated_from", Type = ColType.Text },
        ["३.५ तपाईको परिवारको यहाको बसोवास कस्तो हो ?"] = new() { Table = "household", Column = "residence_type", Type = ColType.Text },
        ["३.६ यदि अस्थायी हो भने तपाईको स्थायी बसोबास ठेगाना कहाँ पर्दछ ?"] = new() { Table = "household", Column = "permanent_address", Type = ColType.Text },
        ["३.७ यसै नपाको अन्य वडामा भए कुन वडामा छ ?"] = new() { Table = "household", Column = "other_ward", Type = ColType.Text },
        ["३.८ यस घरका परिवारमा नागरिकता प्राप्तिको अवस्था के छ ?"] = new() { Table = "household", Column = "citizenship_status", Type = ColType.Text },
        ["३.९ तपाइको परिवारमा जम्मा कति जना सदस्य हुनुहुन्छ ?"] = new() { Table = "household", Column = "family_member_count", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 5: Death
        // ═══════════════════════════════════════════════════
        ["५.१.  तपाईको परिवारमा गत १ वर्ष भित्र कसैको मृत्यु भएको छ ?"] = new() { Table = "household", Column = "death_in_last_year", Type = ColType.Text },
        ["५.२. मृत्यु भएका मानिसको संख्या छ ?"] = new() { Table = "household", Column = "death_count", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 6: Livestock insurance
        // ═══════════════════════════════════════════════════
        ["६.१ पशुपन्छी विमा गर्नु भएको छ"] = new() { Table = "household", Column = "livestock_insured", Type = ColType.Text },
        ["६.१.१ कति वटा पशु पन्छीको गर्नु भएको छ ?"] = new() { Table = "household", Column = "livestock_insured_count", Type = ColType.Double },
        ["सबै भन्दा नजिकको बजारको नाम लेख्\u200Dनुहोस ।"] = new() { Table = "household", Column = "nearest_market_name", Type = ColType.Text },

        // ═══════════════════════════════════════════════════
        // Section 7: Facilities
        // ═══════════════════════════════════════════════════
        ["७.१. तपाईको घरबाट हिडेर नजिकको बजार पुग्न कति समय लाग्छ ?"] = new() { Table = "household", Column = "market_travel_time", Type = ColType.Text },
        ["७.२. घरमा कोहि विरामी परेमा सबैभन्दा पहिले कहाँ जानु हुन्छ ?"] = new() { Table = "household", Column = "first_health_visit", Type = ColType.Text },
        ["७.३. तपाईको घरबाट सरकारी अस्पताल पुग्न कति समय लाग्छ ?"] = new() { Table = "household", Column = "hospital_travel_time", Type = ColType.Text },
        ["७.४ . तपाईको घरमा १६ बर्ष मुनिको (बालबालिका) कोही घरायसी काम गर्न बसेको छ ?"] = new() { Table = "household", Column = "child_labor", Type = ColType.Text },
        ["७.५. तपाईको घरमा खानेपानीको मुख्य श्रोत के हो ?"] = new() { Table = "household", Column = "drinking_water_source", Type = ColType.Text },
        ["७.६. खानेपानीको अन्य श्रोत भए उल्लेख गर्नुहोस ।"] = new() { Table = "household", Column = "drinking_water_other", Type = ColType.Text },
        ["७.७. तपाईको घरमा खाना पकाउन प्रयोग गर्ने मुख्य इन्धन के हो ?"] = new() { Table = "household", Column = "cooking_fuel_primary", Type = ColType.Text },
        ["७.७.१ तपाईको घरमा खाना पकाउन प्रयोग गर्ने दोस्रो मुख्य इन्धन के हो ?"] = new() { Table = "household", Column = "cooking_fuel_secondary", Type = ColType.Text },
        ["७.८. तपाइको घरको चुलो कस्तो  प्रकारको छ"] = new() { Table = "household", Column = "stove_type", Type = ColType.Text },
        ["७.९ तपाईको घरमा प्रयोग हुने बत्तिको श्रोत के हो ?"] = new() { Table = "household", Column = "light_source", Type = ColType.Text },
        ["बत्तिको लागि अन्य श्रोत भए उल्लेख गर्नुहोस ।"] = new() { Table = "household", Column = "light_source_other", Type = ColType.Text },
        ["७.१० तपाइँले घर आगनबाट निस्कने ठोस फोहोरलाई कसरी व्यवस्थापन गर्नु भएको छ ?"] = new() { Table = "household", Column = "waste_management", Type = ColType.Text },
        ["७.११. तपाईको परिवारले प्रयोग गर्ने शौचालय कस्तो छ ?"] = new() { Table = "household", Column = "toilet_type", Type = ColType.Text },

        // 7.12: Assets (multi-select 0/1)
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?"] = new() { Table = "household", Column = "assets_summary", Type = ColType.Text },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/रेडियो"] = new() { Table = "household", Column = "asset_radio", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/साइकल"] = new() { Table = "household", Column = "asset_bicycle", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/टेलिभिजन"] = new() { Table = "household", Column = "asset_television", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/टेलिफोन, मोबाईल फोन"] = new() { Table = "household", Column = "asset_phone", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/कम्प्यूटर"] = new() { Table = "household", Column = "asset_computer", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/ईन्टरनेट"] = new() { Table = "household", Column = "asset_internet", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/मोटरसाईकल"] = new() { Table = "household", Column = "asset_motorcycle", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/मोटर, कार"] = new() { Table = "household", Column = "asset_car", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/रेफ्रिजरेटर"] = new() { Table = "household", Column = "asset_refrigerator", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/ट्रयाक्टर, पावरटिलर"] = new() { Table = "household", Column = "asset_tractor", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/बस, ट्रक आदी"] = new() { Table = "household", Column = "asset_bus_truck", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/टिप्पर, ट्रक, गाडि"] = new() { Table = "household", Column = "asset_tipper_truck", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/डोजर, एक्सक्याभेटर (JCB)"] = new() { Table = "household", Column = "asset_excavator", Type = ColType.SmallInt },
        ["७.१२.  कुन कुन सुविधाहरु परिवारले उपभोग गरेको छ ?/कुनै पनि छैन"] = new() { Table = "household", Column = "asset_none", Type = ColType.SmallInt },

        // 7.13-7.14: Education
        ["७.१३ तपाइको घरबाट बालबालिकाहरु विद्यालय जान कति समय लाग्छ ?"] = new() { Table = "household", Column = "school_travel_time", Type = ColType.Text },
        ["७.१४ तपाइको बालबालिकाहरु कस्तो प्रकारको शैक्षिक संस्थामा अध्ययन गर्छन ?"] = new() { Table = "household", Column = "school_type_summary", Type = ColType.Text },
        ["७.१४ तपाइको बालबालिकाहरु कस्तो प्रकारको शैक्षिक संस्थामा अध्ययन गर्छन ?/सरकारी"] = new() { Table = "household", Column = "school_type_govt", Type = ColType.SmallInt },
        ["७.१४ तपाइको बालबालिकाहरु कस्तो प्रकारको शैक्षिक संस्थामा अध्ययन गर्छन ?/निजी"] = new() { Table = "household", Column = "school_type_private", Type = ColType.SmallInt },
        ["७.१४ तपाइको बालबालिकाहरु कस्तो प्रकारको शैक्षिक संस्थामा अध्ययन गर्छन ?/मदरसा"] = new() { Table = "household", Column = "school_type_madrasa", Type = ColType.SmallInt },

// ═══════════════════════════════════════════════════
// CONTINUED IN PART 6B...
// ═══════════════════════════════════════════════════
                // ═══════════════════════════════════════════════════
        // Section 8: Agriculture land
        // ═══════════════════════════════════════════════════
        ["८.  तपाईको  परिवारले कृषि कार्यको लागि जग्गा उपभोग गरेको छ कि छैन ?"] = new() { Table = "household", Column = "has_agriculture_land", Type = ColType.Text },
        ["८ .१. जग्गा उपभोग गर्नु भएको भए आफ्नो परिवारको नाममा कति जग्गा छ ? ( कठ्ठामा भन्नुहोस) ।"] = new() { Table = "household", Column = "own_land_kattha", Type = ColType.Double },
        ["८ .२. आफ्नो  बाहेक अरूको जग्गा पनि कमाउनु भएको भए कति छ ? (कठ्ठामा भन्नुहोस ) ।"] = new() { Table = "household", Column = "others_land_kattha", Type = ColType.Double },
        ["८.३. अन्य स्वामित्वको जग्गा पनि कमाउनु भएको भए कति छ ? (कठ्ठामा भन्नुहोस) ।"] = new() { Table = "household", Column = "other_ownership_kattha", Type = ColType.Double },
        ["८. ४.  तपाईको परिवारले उपभोग गर्ने जमिन मध्ये खेत कति छ ?(कठ्ठामा भन्नुहोस)"] = new() { Table = "household", Column = "khet_area_kattha", Type = ColType.Double },
        ["८.५.  तपाईको परिवारले उपभोग गर्ने जमिन मध्ये बारी कति छ ? (कठ्ठामा भन्नुहोस)"] = new() { Table = "household", Column = "bari_area_kattha", Type = ColType.Double },
        ["८.६. तपाईको परिवारसंग भएकाे  जमिन मध्ये बाझो जमिन कति छ ? (कठ्ठामा भन्नुहोस)"] = new() { Table = "household", Column = "fallow_land_kattha", Type = ColType.Double },
        ["८ .७. तपाईको खेतमा सिंचाई सुविधा छ कि छैन ?"] = new() { Table = "household", Column = "irrigation_status", Type = ColType.Text },
        ["८.८ सिँचाइको माध्यम कुन हो ?"] = new() { Table = "household", Column = "irrigation_medium_summary", Type = ColType.Text },
        ["८.८ सिँचाइको माध्यम कुन हो ?/नहर/कुलो"] = new() { Table = "household", Column = "irrigation_canal", Type = ColType.SmallInt },
        ["८.८ सिँचाइको माध्यम कुन हो ?/स्यालो ट्यूवबेल"] = new() { Table = "household", Column = "irrigation_tubewell", Type = ColType.SmallInt },
        ["८.८ सिँचाइको माध्यम कुन हो ?/विद्युत"] = new() { Table = "household", Column = "irrigation_electric", Type = ColType.SmallInt },
        ["८.९ तपाइको परिवारले खेतीपातीका लागि कुन माध्यमको प्रयोग गर्छ ?"] = new() { Table = "household", Column = "farming_method", Type = ColType.Text },

        // ═══════════════════════════════════════════════════
        // Section 9.1: Cereal crops + production
        // ═══════════════════════════════════════════════════
        ["९ तपाइको घरमा अन्\u200Dनबाली उत्पादन हुन्छ कि हुदैन ?"] = new() { Table = "household", Column = "has_cereal_crops", Type = ColType.Text },
        ["९.१.१ अन्नबाली उत्पादन बिवरण"] = new() { Table = "household", Column = "cereal_crop_summary", Type = ColType.Text },
        ["९.१.१ अन्नबाली उत्पादन बिवरण/धान"] = new() { Table = "household", Column = "cereal_rice", Type = ColType.SmallInt },
        ["९.१.१ अन्नबाली उत्पादन बिवरण/मकै"] = new() { Table = "household", Column = "cereal_maize", Type = ColType.SmallInt },
        ["९.१.१ अन्नबाली उत्पादन बिवरण/गहुँ"] = new() { Table = "household", Column = "cereal_wheat", Type = ColType.SmallInt },
        ["९.१.१ अन्नबाली उत्पादन बिवरण/जौ"] = new() { Table = "household", Column = "cereal_barley", Type = ColType.SmallInt },

        // Rice (no suffix)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।"] = new() { Table = "household", Column = "rice_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।"] = new() { Table = "household", Column = "rice_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।"] = new() { Table = "household", Column = "rice_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?"] = new() { Table = "household", Column = "rice_deficit_rupees", Type = ColType.Double },

        // Maize (_2)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_2"] = new() { Table = "household", Column = "maize_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_2"] = new() { Table = "household", Column = "maize_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_2"] = new() { Table = "household", Column = "maize_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_2"] = new() { Table = "household", Column = "maize_deficit_rupees", Type = ColType.Double },

        // Wheat (_3)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_3"] = new() { Table = "household", Column = "wheat_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_3"] = new() { Table = "household", Column = "wheat_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_3"] = new() { Table = "household", Column = "wheat_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_3"] = new() { Table = "household", Column = "wheat_deficit_rupees", Type = ColType.Double },

        // Barley (_4)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_4"] = new() { Table = "household", Column = "barley_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_4"] = new() { Table = "household", Column = "barley_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_4"] = new() { Table = "household", Column = "barley_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_4"] = new() { Table = "household", Column = "barley_deficit_rupees", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 9.2: Pulses (8 types, suffixes _5 to _12)
        // ═══════════════════════════════════════════════════
        ["तपाइको घरमा दलहन बाली उत्पादन हुन्छ कि हुदैन ?"] = new() { Table = "household", Column = "has_pulse_crops", Type = ColType.Text },
        ["९.२.१. दलहन बालीको सूची"] = new() { Table = "household", Column = "pulse_crop_summary", Type = ColType.Text },
        ["९.२.१. दलहन बालीको सूची/कालो दाल"] = new() { Table = "household", Column = "pulse_black_dal", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/रहर"] = new() { Table = "household", Column = "pulse_rahar", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/मसुरो"] = new() { Table = "household", Column = "pulse_masuro", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/चना"] = new() { Table = "household", Column = "pulse_chana", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/सिमि"] = new() { Table = "household", Column = "pulse_simi", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/भट्टमास"] = new() { Table = "household", Column = "pulse_bhattmas", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/मुङ्ग"] = new() { Table = "household", Column = "pulse_mung", Type = ColType.SmallInt },
        ["९.२.१. दलहन बालीको सूची/खेसरीको दाल"] = new() { Table = "household", Column = "pulse_khesari", Type = ColType.SmallInt },

        // Pulse 1: कालो दाल (_5)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_5"] = new() { Table = "household", Column = "pulse_1_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_5"] = new() { Table = "household", Column = "pulse_1_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_5"] = new() { Table = "household", Column = "pulse_1_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_5"] = new() { Table = "household", Column = "pulse_1_deficit_rupees", Type = ColType.Double },

        // Pulse 2: रहर (_6)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_6"] = new() { Table = "household", Column = "pulse_2_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_6"] = new() { Table = "household", Column = "pulse_2_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_6"] = new() { Table = "household", Column = "pulse_2_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_6"] = new() { Table = "household", Column = "pulse_2_deficit_rupees", Type = ColType.Double },

        // Pulse 3: मसुरो (_7)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_7"] = new() { Table = "household", Column = "pulse_3_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_7"] = new() { Table = "household", Column = "pulse_3_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_7"] = new() { Table = "household", Column = "pulse_3_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_7"] = new() { Table = "household", Column = "pulse_3_deficit_rupees", Type = ColType.Double },

        // Pulse 4: चना (_8)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_8"] = new() { Table = "household", Column = "pulse_4_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_8"] = new() { Table = "household", Column = "pulse_4_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_8"] = new() { Table = "household", Column = "pulse_4_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_8"] = new() { Table = "household", Column = "pulse_4_deficit_rupees", Type = ColType.Double },

        // Pulse 5: सिमि (_9)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_9"] = new() { Table = "household", Column = "pulse_5_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_9"] = new() { Table = "household", Column = "pulse_5_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_9"] = new() { Table = "household", Column = "pulse_5_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_9"] = new() { Table = "household", Column = "pulse_5_deficit_rupees", Type = ColType.Double },

        // Pulse 6: भट्टमास (_10)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_10"] = new() { Table = "household", Column = "pulse_6_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_10"] = new() { Table = "household", Column = "pulse_6_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_10"] = new() { Table = "household", Column = "pulse_6_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_10"] = new() { Table = "household", Column = "pulse_6_deficit_rupees", Type = ColType.Double },

        // Pulse 7: मुङ्ग (_11)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_11"] = new() { Table = "household", Column = "pulse_7_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_11"] = new() { Table = "household", Column = "pulse_7_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_11"] = new() { Table = "household", Column = "pulse_7_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_11"] = new() { Table = "household", Column = "pulse_7_deficit_rupees", Type = ColType.Double },

        // Pulse 8: खेसरी (_12)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_12"] = new() { Table = "household", Column = "pulse_8_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_12"] = new() { Table = "household", Column = "pulse_8_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_12"] = new() { Table = "household", Column = "pulse_8_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_12"] = new() { Table = "household", Column = "pulse_8_deficit_rupees", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 9.3: Oilseeds (4 types, suffixes _13 to _16)
        // ═══════════════════════════════════════════════════
        ["तपाइको घरमा तेलहन उत्पादन हुन्छ कि हुदैन ?"] = new() { Table = "household", Column = "has_oilseed_crops", Type = ColType.Text },
        ["९.३.१. तेलहन बालीको सूची"] = new() { Table = "household", Column = "oilseed_crop_summary", Type = ColType.Text },
        ["९.३.१. तेलहन बालीको सूची/तोरी वा सर्स्यु"] = new() { Table = "household", Column = "oilseed_tori", Type = ColType.SmallInt },
        ["९.३.१. तेलहन बालीको सूची/आलस  वा तिल"] = new() { Table = "household", Column = "oilseed_til", Type = ColType.SmallInt },
        ["९.३.१. तेलहन बालीको सूची/सुर्यमुखी"] = new() { Table = "household", Column = "oilseed_sunflower", Type = ColType.SmallInt },
        ["९.३.१. तेलहन बालीको सूची/अन्य"] = new() { Table = "household", Column = "oilseed_other", Type = ColType.SmallInt },

        // Oilseed 1: तोरी (_13)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_13"] = new() { Table = "household", Column = "oilseed_1_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_13"] = new() { Table = "household", Column = "oilseed_1_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_13"] = new() { Table = "household", Column = "oilseed_1_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_13"] = new() { Table = "household", Column = "oilseed_1_deficit_rupees", Type = ColType.Double },

        // Oilseed 2: तिल (_14)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_14"] = new() { Table = "household", Column = "oilseed_2_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_14"] = new() { Table = "household", Column = "oilseed_2_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_14"] = new() { Table = "household", Column = "oilseed_2_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_14"] = new() { Table = "household", Column = "oilseed_2_deficit_rupees", Type = ColType.Double },

        // Oilseed 3: सुर्यमुखी (_15)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_15"] = new() { Table = "household", Column = "oilseed_3_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_15"] = new() { Table = "household", Column = "oilseed_3_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_15"] = new() { Table = "household", Column = "oilseed_3_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_15"] = new() { Table = "household", Column = "oilseed_3_deficit_rupees", Type = ColType.Double },

        // Oilseed 4: अन्य (_16)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_16"] = new() { Table = "household", Column = "oilseed_4_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_16"] = new() { Table = "household", Column = "oilseed_4_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_16"] = new() { Table = "household", Column = "oilseed_4_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_16"] = new() { Table = "household", Column = "oilseed_4_deficit_rupees", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 9.4: Cash crops (6 types, suffixes _17 to _22)
        // ═══════════════════════════════════════════════════
        ["तपाइको घरमा नगदेबाली उत्पादन हुन्छ कि हुदैन ?"] = new() { Table = "household", Column = "has_cash_crops", Type = ColType.Text },
        ["९.४.१. नगदेबालीको सूची"] = new() { Table = "household", Column = "cash_crop_summary", Type = ColType.Text },
        ["९.४.१. नगदेबालीको सूची/अदुवा, बेसार"] = new() { Table = "household", Column = "cash_ginger_turmeric", Type = ColType.SmallInt },
        ["९.४.१. नगदेबालीको सूची/प्याज, लसुन"] = new() { Table = "household", Column = "cash_onion_garlic", Type = ColType.SmallInt },
        ["९.४.१. नगदेबालीको सूची/च्याउ"] = new() { Table = "household", Column = "cash_mushroom", Type = ColType.SmallInt },
        ["९.४.१. नगदेबालीको सूची/उखु"] = new() { Table = "household", Column = "cash_sugarcane", Type = ColType.SmallInt },
        ["९.४.१. नगदेबालीको सूची/खुर्सानी"] = new() { Table = "household", Column = "cash_chili", Type = ColType.SmallInt },
        ["९.४.१. नगदेबालीको सूची/बदाम"] = new() { Table = "household", Column = "cash_peanut", Type = ColType.SmallInt },

        // Cash 1: अदुवा (_17), Cash 2: प्याज (_18), ... Cash 6: बदाम (_22)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_17"] = new() { Table = "household", Column = "cash_1_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_17"] = new() { Table = "household", Column = "cash_1_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_17"] = new() { Table = "household", Column = "cash_1_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_17"] = new() { Table = "household", Column = "cash_1_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_18"] = new() { Table = "household", Column = "cash_2_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_18"] = new() { Table = "household", Column = "cash_2_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_18"] = new() { Table = "household", Column = "cash_2_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_18"] = new() { Table = "household", Column = "cash_2_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_19"] = new() { Table = "household", Column = "cash_3_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_19"] = new() { Table = "household", Column = "cash_3_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_19"] = new() { Table = "household", Column = "cash_3_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_19"] = new() { Table = "household", Column = "cash_3_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_20"] = new() { Table = "household", Column = "cash_4_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_20"] = new() { Table = "household", Column = "cash_4_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_20"] = new() { Table = "household", Column = "cash_4_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_20"] = new() { Table = "household", Column = "cash_4_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_21"] = new() { Table = "household", Column = "cash_5_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_21"] = new() { Table = "household", Column = "cash_5_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_21"] = new() { Table = "household", Column = "cash_5_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_21"] = new() { Table = "household", Column = "cash_5_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_22"] = new() { Table = "household", Column = "cash_6_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_22"] = new() { Table = "household", Column = "cash_6_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_22"] = new() { Table = "household", Column = "cash_6_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_22"] = new() { Table = "household", Column = "cash_6_deficit_rupees", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 9.5: Fruits (8 types, suffixes _23 to _30)
        // ═══════════════════════════════════════════════════
        ["तपाइको घरमा फलफूल बाली उत्पादन हुन्छ कि हुदैन ?"] = new() { Table = "household", Column = "has_fruit_crops", Type = ColType.Text },
        ["९.५.१. फलफुल खेतीको सूची"] = new() { Table = "household", Column = "fruit_crop_summary", Type = ColType.Text },
        ["९.५.१. फलफुल खेतीको सूची/आँप"] = new() { Table = "household", Column = "fruit_mango", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/कागती"] = new() { Table = "household", Column = "fruit_lemon", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/भोगटे"] = new() { Table = "household", Column = "fruit_pomelo", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/अनार"] = new() { Table = "household", Column = "fruit_pomegranate", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/केरा"] = new() { Table = "household", Column = "fruit_banana", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/अम्बा"] = new() { Table = "household", Column = "fruit_amba", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/लिचि"] = new() { Table = "household", Column = "fruit_lichi", Type = ColType.SmallInt },
        ["९.५.१. फलफुल खेतीको सूची/सरिफा"] = new() { Table = "household", Column = "fruit_sarifa", Type = ColType.SmallInt },

        // Fruit 1-8 production (_23 to _30)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_23"] = new() { Table = "household", Column = "fruit_1_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_23"] = new() { Table = "household", Column = "fruit_1_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_23"] = new() { Table = "household", Column = "fruit_1_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_23"] = new() { Table = "household", Column = "fruit_1_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_24"] = new() { Table = "household", Column = "fruit_2_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_24"] = new() { Table = "household", Column = "fruit_2_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_24"] = new() { Table = "household", Column = "fruit_2_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_24"] = new() { Table = "household", Column = "fruit_2_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_25"] = new() { Table = "household", Column = "fruit_3_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_25"] = new() { Table = "household", Column = "fruit_3_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_25"] = new() { Table = "household", Column = "fruit_3_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_25"] = new() { Table = "household", Column = "fruit_3_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_26"] = new() { Table = "household", Column = "fruit_4_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_26"] = new() { Table = "household", Column = "fruit_4_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_26"] = new() { Table = "household", Column = "fruit_4_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_26"] = new() { Table = "household", Column = "fruit_4_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_27"] = new() { Table = "household", Column = "fruit_5_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_27"] = new() { Table = "household", Column = "fruit_5_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_27"] = new() { Table = "household", Column = "fruit_5_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_27"] = new() { Table = "household", Column = "fruit_5_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_28"] = new() { Table = "household", Column = "fruit_6_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_28"] = new() { Table = "household", Column = "fruit_6_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_28"] = new() { Table = "household", Column = "fruit_6_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_28"] = new() { Table = "household", Column = "fruit_6_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_29"] = new() { Table = "household", Column = "fruit_7_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_29"] = new() { Table = "household", Column = "fruit_7_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_29"] = new() { Table = "household", Column = "fruit_7_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_29"] = new() { Table = "household", Column = "fruit_7_deficit_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_30"] = new() { Table = "household", Column = "fruit_8_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_30"] = new() { Table = "household", Column = "fruit_8_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_30"] = new() { Table = "household", Column = "fruit_8_sales_rupees", Type = ColType.Double },
        ["यदि अपुग हुने भए वर्षमा कति रुपैयाँको किन्\u200Dनु हुन्छ ?_30"] = new() { Table = "household", Column = "fruit_8_deficit_rupees", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 9.6: Herbs (9 types, suffixes _31 to _39)
        // NOTE: Herbs have only 3 metrics (area, production, sales)
        //       No "deficit" column for herbs
        // ═══════════════════════════════════════════════════
        ["तपाइको घरमा जडिबुटी उत्पादन हुन्छ कि हुदैन ?"] = new() { Table = "household", Column = "has_herb_crops", Type = ColType.Text },
        ["९.६.१. जडिबुटीको नाम सूची"] = new() { Table = "household", Column = "herb_crop_summary", Type = ColType.Text },
        ["९.६.१. जडिबुटीको नाम सूची/हर्रो बर्रो"] = new() { Table = "household", Column = "herb_harro_barro", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/गुर्जो"] = new() { Table = "household", Column = "herb_gurjo", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/तुलसी"] = new() { Table = "household", Column = "herb_tulsi", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/अमला"] = new() { Table = "household", Column = "herb_amla", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/सर्पगन्धा"] = new() { Table = "household", Column = "herb_sarpagandha", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/अजवाइन"] = new() { Table = "household", Column = "herb_ajwain", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/सुतली"] = new() { Table = "household", Column = "herb_sutli", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/घ्यूकुमारी"] = new() { Table = "household", Column = "herb_aloe_vera", Type = ColType.SmallInt },
        ["९.६.१. जडिबुटीको नाम सूची/बोझो"] = new() { Table = "household", Column = "herb_bojho", Type = ColType.SmallInt },

        // Herb 1-9 production (_31 to _39, 3 metrics each)
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_31"] = new() { Table = "household", Column = "herb_1_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_31"] = new() { Table = "household", Column = "herb_1_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_31"] = new() { Table = "household", Column = "herb_1_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_32"] = new() { Table = "household", Column = "herb_2_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_32"] = new() { Table = "household", Column = "herb_2_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_32"] = new() { Table = "household", Column = "herb_2_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_33"] = new() { Table = "household", Column = "herb_3_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_33"] = new() { Table = "household", Column = "herb_3_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_33"] = new() { Table = "household", Column = "herb_3_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_34"] = new() { Table = "household", Column = "herb_4_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_34"] = new() { Table = "household", Column = "herb_4_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_34"] = new() { Table = "household", Column = "herb_4_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_35"] = new() { Table = "household", Column = "herb_5_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_35"] = new() { Table = "household", Column = "herb_5_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_35"] = new() { Table = "household", Column = "herb_5_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_36"] = new() { Table = "household", Column = "herb_6_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_36"] = new() { Table = "household", Column = "herb_6_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_36"] = new() { Table = "household", Column = "herb_6_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_37"] = new() { Table = "household", Column = "herb_7_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_37"] = new() { Table = "household", Column = "herb_7_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_37"] = new() { Table = "household", Column = "herb_7_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_38"] = new() { Table = "household", Column = "herb_8_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_38"] = new() { Table = "household", Column = "herb_8_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_38"] = new() { Table = "household", Column = "herb_8_sales_rupees", Type = ColType.Double },
        ["खेती गरिने क्षेत्रफल कति हो कठ्ठामा भन्\u200Dनु होला ।_39"] = new() { Table = "household", Column = "herb_9_area", Type = ColType.Double },
        ["कति परिमाणमा उत्पादन हुन्छ केजीमा भन्नुहोला ।_39"] = new() { Table = "household", Column = "herb_9_production_kg", Type = ColType.Double },
        ["वार्षिकरुपमा कति विक्रि गर्नु हुन्छ रुपैयाँमा भन्\u200Dनु होला ।_39"] = new() { Table = "household", Column = "herb_9_sales_rupees", Type = ColType.Double },

                // ═══════════════════════════════════════════════════
        // Section 10: Livestock
        // ═══════════════════════════════════════════════════
        ["१०. परिवारले कुनै चौपाया तथा पशुपंक्षी पालेको छ ?"] = new() { Table = "household", Column = "has_livestock", Type = ColType.Text },

        // 10.1: Dairy livestock summary + multi-select
        ["१०.१.१. दुधजन्य चौपायाको सूची"] = new() { Table = "household", Column = "dairy_summary", Type = ColType.Text },
        ["१०.१.१. दुधजन्य चौपायाको सूची/गाई"] = new() { Table = "household", Column = "dairy_cow", Type = ColType.SmallInt },
        ["१०.१.१. दुधजन्य चौपायाको सूची/भैसी"] = new() { Table = "household", Column = "dairy_buffalo", Type = ColType.SmallInt },
        ["१०.१.१. दुधजन्य चौपायाको सूची/अन्य दूधका लागि पालिएका पशु भएमा"] = new() { Table = "household", Column = "dairy_other", Type = ColType.SmallInt },

        // Cow data (no suffix)
        ["संख्या कति वटा छन"] = new() { Table = "household", Column = "cow_count", Type = ColType.Double },
        ["दैनिक कति लिटर दूध उत्पादन हुन्छ ?"] = new() { Table = "household", Column = "cow_milk_daily", Type = ColType.Double },
        ["कति दूध लिटर विक्रि गर्नु हुन्छ ?"] = new() { Table = "household", Column = "cow_milk_sold", Type = ColType.Double },
        ["दहि कति लिटर विक्रि गर्नु हुन्छ ?"] = new() { Table = "household", Column = "cow_curd_sold", Type = ColType.Double },
        ["वार्षक औषतमा कति रुपैयाँ आम्दानी हुन्छ ?"] = new() { Table = "household", Column = "cow_annual_income", Type = ColType.Double },

        // Buffalo data (_2)
        ["संख्या कति वटा छन_2"] = new() { Table = "household", Column = "buffalo_count", Type = ColType.Double },
        ["दैनिक कति लिटर दूध उत्पादन हुन्छ ?_2"] = new() { Table = "household", Column = "buffalo_milk_daily", Type = ColType.Double },
        ["कति दूध लिटर विक्रि गर्नु हुन्छ ?_2"] = new() { Table = "household", Column = "buffalo_milk_sold", Type = ColType.Double },
        ["दहि कति लिटर विक्रि गर्नु हुन्छ ?_2"] = new() { Table = "household", Column = "buffalo_curd_sold", Type = ColType.Double },
        ["वार्षक औषतमा कति रुपैयाँ आम्दानी हुन्छ ?_2"] = new() { Table = "household", Column = "buffalo_annual_income", Type = ColType.Double },

        // Other dairy (_3)
        ["संख्या कति वटा छन_3"] = new() { Table = "household", Column = "other_dairy_count", Type = ColType.Double },
        ["दैनिक कति लिटर दूध उत्पादन हुन्छ ?_3"] = new() { Table = "household", Column = "other_dairy_milk_daily", Type = ColType.Double },
        ["कति दूध लिटर विक्रि गर्नु हुन्छ ?_3"] = new() { Table = "household", Column = "other_dairy_milk_sold", Type = ColType.Double },
        ["दहि कति लिटर विक्रि गर्नु हुन्छ ?_3"] = new() { Table = "household", Column = "other_dairy_curd_sold", Type = ColType.Double },
        ["वार्षक औषतमा कति रुपैयाँ आम्दानी हुन्छ ?_3"] = new() { Table = "household", Column = "other_dairy_annual_income", Type = ColType.Double },

        // 10.2: Meat livestock summary + multi-select
        ["१०.२.१. मासु तथा अन्य प्रयोजनको लागि पालिने चौपायको सूची"] = new() { Table = "household", Column = "meat_summary", Type = ColType.Text },
        ["१०.२.१. मासु तथा अन्य प्रयोजनको लागि पालिने चौपायको सूची/खसी, बोका, भेडा, बाख्रा"] = new() { Table = "household", Column = "meat_goat_sheep", Type = ColType.SmallInt },
        ["१०.२.१. मासु तथा अन्य प्रयोजनको लागि पालिने चौपायको सूची/रागा, पाडा, पाडी"] = new() { Table = "household", Column = "meat_calf", Type = ColType.SmallInt },
        ["१०.२.१. मासु तथा अन्य प्रयोजनको लागि पालिने चौपायको सूची/सुगुर, बंगुर, बदेल"] = new() { Table = "household", Column = "meat_pig", Type = ColType.SmallInt },
        ["१०.२.१. मासु तथा अन्य प्रयोजनको लागि पालिने चौपायको सूची/अन्य"] = new() { Table = "household", Column = "meat_other", Type = ColType.SmallInt },

        // Goat/sheep data (no suffix)
        ["संख्या कति छ ?"] = new() { Table = "household", Column = "goat_count", Type = ColType.Double },
        ["वार्षिक औषतमा कति वटा विक्रि गर्नु हुन्छ ?"] = new() { Table = "household", Column = "goat_annual_sold", Type = ColType.Double },
        ["वार्षिक औषतमा कति रुपैयाँ कमाउनु हुन्छ ?"] = new() { Table = "household", Column = "goat_annual_income", Type = ColType.Double },

        // Calf data (_2)
        ["संख्या कति छ ?_2"] = new() { Table = "household", Column = "calf_count", Type = ColType.Double },
        ["वार्षिक औषतमा कति वटा विक्रि गर्नु हुन्छ ?_2"] = new() { Table = "household", Column = "calf_annual_sold", Type = ColType.Double },
        ["वार्षिक औषतमा कति रुपैयाँ कमाउनु हुन्छ ?_2"] = new() { Table = "household", Column = "calf_annual_income", Type = ColType.Double },

        // Pig data (_3)
        ["संख्या कति छ ?_3"] = new() { Table = "household", Column = "pig_count", Type = ColType.Double },
        ["वार्षिक औषतमा कति वटा विक्रि गर्नु हुन्छ ?_3"] = new() { Table = "household", Column = "pig_annual_sold", Type = ColType.Double },
        ["वार्षिक औषतमा कति रुपैयाँ कमाउनु हुन्छ ?_3"] = new() { Table = "household", Column = "pig_annual_income", Type = ColType.Double },

        // Other meat (_4)
        ["संख्या कति छ ?_4"] = new() { Table = "household", Column = "other_meat_count", Type = ColType.Double },
        ["वार्षिक औषतमा कति वटा विक्रि गर्नु हुन्छ ?_4"] = new() { Table = "household", Column = "other_meat_annual_sold", Type = ColType.Double },
        ["वार्षिक औषतमा कति रुपैयाँ कमाउनु हुन्छ ?_4"] = new() { Table = "household", Column = "other_meat_annual_income", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 11: Poultry
        // ═══════════════════════════════════════════════════
        ["११.१. पंक्षीहरूको सूची"] = new() { Table = "household", Column = "poultry_summary", Type = ColType.Text },
        ["११.१. पंक्षीहरूको सूची/कुखुरा"] = new() { Table = "household", Column = "poultry_chicken", Type = ColType.SmallInt },
        ["११.१. पंक्षीहरूको सूची/हाँस"] = new() { Table = "household", Column = "poultry_duck", Type = ColType.SmallInt },
        ["११.१. पंक्षीहरूको सूची/परेवा"] = new() { Table = "household", Column = "poultry_pigeon", Type = ColType.SmallInt },
        ["११.१. पंक्षीहरूको सूची/बट्टाई"] = new() { Table = "household", Column = "poultry_battai", Type = ColType.SmallInt },
        ["११.२. पंक्षीको संख्या"] = new() { Table = "household", Column = "poultry_count", Type = ColType.Double },
        ["११.३. वार्षिक रूपमा अण्डा उत्पादन (गोटामा)"] = new() { Table = "household", Column = "poultry_egg_annual", Type = ColType.Double },
        ["११.४.  वार्षिक रूमा पंक्षीको मासु बिक्री (केजीमा)"] = new() { Table = "household", Column = "poultry_meat_sold_kg", Type = ColType.Double },
        ["११.५. मासु र अणडा बिक्रबाट हुने वार्षिक आम्दानी  (रू हजारमा लेख्नुहोस)"] = new() { Table = "household", Column = "poultry_annual_income", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 12: Fish farming
        // ═══════════════════════════════════════════════════
        ["१२. तपाईको घरमा माछा पालन गर्नु भएको छ ?"] = new() { Table = "household", Column = "has_fish_farming", Type = ColType.Text },
        ["१२.१.  पोखरी कतिवटा छन् ?"] = new() { Table = "household", Column = "fish_pond_count", Type = ColType.Double },
        ["१२.२.  जम्मा पोखरीको क्षेत्रफल कति छ ?"] = new() { Table = "household", Column = "fish_pond_area", Type = ColType.Double },
        ["१२.३.  वार्षिक माछा उत्पादन (केजीमा)"] = new() { Table = "household", Column = "fish_production_kg", Type = ColType.Double },
        ["१२.४ वार्षिक माछा विक्रिबाट कति आम्दानी हुन्छ ? रुपैयाँमा भन्नु होला"] = new() { Table = "household", Column = "fish_annual_income", Type = ColType.Double },
        ["१२.५ माछापालन व्यवसायका लागि कहिँकतैबाट अनुदान लिनु भएको छ कि छैन ?"] = new() { Table = "household", Column = "fish_subsidy", Type = ColType.Text },

        // ═══════════════════════════════════════════════════
        // Section 13: Beekeeping
        // ═══════════════════════════════════════════════════
        ["१३. घरमा मौरीपालन गर्नु भएको छ ?"] = new() { Table = "household", Column = "has_beekeeping", Type = ColType.Text },
        ["१३.१. कति घार छन् ?"] = new() { Table = "household", Column = "bee_hive_count", Type = ColType.Double },
        ["१३.२. वार्षिक रूपमा मह कति केजी उत्पादन गर्नु हुन्छ ?"] = new() { Table = "household", Column = "honey_production_kg", Type = ColType.Double },
        ["१३.३. मौरीको मह बिक्रिबाट वार्षिक कति रूपैया आम्दानी गर्नु हुन्छ ? (रू हजारमा लेख्नु हाेस । )"] = new() { Table = "household", Column = "honey_annual_income", Type = ColType.Double },
        ["मौरीपालन व्यवसायका लागि कहिँकतैबाट अनुदान लिनु भएको छ ?"] = new() { Table = "household", Column = "bee_subsidy", Type = ColType.Text },

// ═══════════════════════════════════════════════════
// CONTINUED IN PART 6D...
// ═══════════════════════════════════════════════════
                // ═══════════════════════════════════════════════════
        // Section 14: Income
        // ═══════════════════════════════════════════════════
        ["१४.१. तरकारीबाट आम्दानी कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "income_vegetables", Type = ColType.Double },
        ["१४.२. श्रमबाट आम्दानी कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "income_labor", Type = ColType.Double },
        ["१४.३. व्यापारबाट आम्दानी कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "income_trade", Type = ColType.Double },
        ["१४.४.  वैदेशिक  रोजगारबाट आम्दानी कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "income_foreign", Type = ColType.Double },
        ["१४.५.  जागिरबाट आम्दानी कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "income_job", Type = ColType.Double },
        ["अन्यबाट आम्दानी कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "income_other", Type = ColType.Double },

        // ═══════════════════════════════════════════════════
        // Section 15: Expenditure
        // ═══════════════════════════════════════════════════
        ["१५.१. खान पिन वा खाद्यान्नमा खर्च कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_food", Type = ColType.Double },
        ["१५.२. लत्ता कपडामा खर्च कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_clothes", Type = ColType.Double },
        ["१५.३.  शिक्षामा खर्च कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_education", Type = ColType.Double },
        ["१५.४. स्वास्थ्यमा खर्च कति हुन्छ ?(वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_health", Type = ColType.Double },
        ["१५.५. चाडपवमा खर्च कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_festivals", Type = ColType.Double },
        ["१५.६. कृषि कार्यमा खर्च कति हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_agriculture", Type = ColType.Double },
        ["१५.७. ईन्धनमा कत्ति खर्च हुन्छ ? (वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_fuel", Type = ColType.Double },
        ["१५.८. मोबाई वा ईन्टरनेटमा कति खर्च हुन्छ ?(वार्षिक रु औषतमा हजारमा लेख्\u200Dनु होला । )"] = new() { Table = "household", Column = "expense_mobile_internet", Type = ColType.Double },
        ["१५.९. परिवारको वार्षिक उत्पादनले  तपाईको परिवारलाई  वर्षमा कति महिना खान पुग्छ ?"] = new() { Table = "household", Column = "food_sufficiency_months", Type = ColType.Text },

        // 15.10: Deficit spending (multi-select)
        ["१५.१०. वर्ष दिन खान पुग्दैन भने, बाकि अवधिको लागि  कसरी खर्च जुटाउनु हुन्छ ?"] = new() { Table = "household", Column = "deficit_spending_summary", Type = ColType.Text },
        ["१५.१०. वर्ष दिन खान पुग्दैन भने, बाकि अवधिको लागि  कसरी खर्च जुटाउनु हुन्छ ?/जागिर"] = new() { Table = "household", Column = "deficit_job", Type = ColType.SmallInt },
        ["१५.१०. वर्ष दिन खान पुग्दैन भने, बाकि अवधिको लागि  कसरी खर्च जुटाउनु हुन्छ ?/ज्याला मजदुरी"] = new() { Table = "household", Column = "deficit_daily_wage", Type = ColType.SmallInt },
        ["१५.१०. वर्ष दिन खान पुग्दैन भने, बाकि अवधिको लागि  कसरी खर्च जुटाउनु हुन्छ ?/बैदेसिक रोजगार"] = new() { Table = "household", Column = "deficit_foreign_job", Type = ColType.SmallInt },
        ["१५.१०. वर्ष दिन खान पुग्दैन भने, बाकि अवधिको लागि  कसरी खर्च जुटाउनु हुन्छ ?/ब्यापार"] = new() { Table = "household", Column = "deficit_trade", Type = ColType.SmallInt },
        ["१५.१०. वर्ष दिन खान पुग्दैन भने, बाकि अवधिको लागि  कसरी खर्च जुटाउनु हुन्छ ?/ऋण सापटी"] = new() { Table = "household", Column = "deficit_loan", Type = ColType.SmallInt },

        // ═══════════════════════════════════════════════════
        // Section 16: Banking & loans
        // ═══════════════════════════════════════════════════
        ["१६.१ परिवारको कुनै सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ ?"] = new() { Table = "household", Column = "has_bank_account", Type = ColType.Text },
        ["१६.२. तपाईको परिवारका सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ भने कहाँ छ ?"] = new() { Table = "household", Column = "bank_account_summary", Type = ColType.Text },
        ["१६.२. तपाईको परिवारका सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ भने कहाँ छ ?/बैंक तथा  वित्तीय संस्था"] = new() { Table = "household", Column = "bank_account_bank", Type = ColType.SmallInt },
        ["१६.२. तपाईको परिवारका सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ भने कहाँ छ ?/सहकारी"] = new() { Table = "household", Column = "bank_account_coop", Type = ColType.SmallInt },
        ["१६.२. तपाईको परिवारका सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ भने कहाँ छ ?/बचत समूह"] = new() { Table = "household", Column = "bank_account_savings", Type = ColType.SmallInt },
        ["१६.२. तपाईको परिवारका सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ भने कहाँ छ ?/साहु महाजन"] = new() { Table = "household", Column = "bank_account_moneylender", Type = ColType.SmallInt },
        ["१६.२. तपाईको परिवारका सदस्यको बैंक तथा वित्तीय संस्थामा खाता छ भने कहाँ छ ?/अन्य"] = new() { Table = "household", Column = "bank_account_other", Type = ColType.SmallInt },
        ["१६.३. तपाईको परिवारले गत १ बर्षमा ऋण लिएको छ कि छैन ?"] = new() { Table = "household", Column = "has_loan", Type = ColType.Text },
        ["१६.४ यदि ऋण लिएको भए जम्मा ऋण कति छ ?"] = new() { Table = "household", Column = "loan_amount", Type = ColType.Text },
        ["१६.५ ऋण कहाँबाट लिनु भएको छ ?"] = new() { Table = "household", Column = "loan_source_summary", Type = ColType.Text },
        ["१६.५ ऋण कहाँबाट लिनु भएको छ ?/बैंकबाट"] = new() { Table = "household", Column = "loan_from_bank", Type = ColType.SmallInt },
        ["१६.५ ऋण कहाँबाट लिनु भएको छ ?/सहकारीबाट"] = new() { Table = "household", Column = "loan_from_coop", Type = ColType.SmallInt },
        ["१६.५ ऋण कहाँबाट लिनु भएको छ ?/साहु महाजनबाट"] = new() { Table = "household", Column = "loan_from_moneylender", Type = ColType.SmallInt },
        ["१६.५ ऋण कहाँबाट लिनु भएको छ ?/आफन्तबाट"] = new() { Table = "household", Column = "loan_from_relatives", Type = ColType.SmallInt },
        ["१६.५.१कस्तो ऋण हो"] = new() { Table = "household", Column = "loan_type", Type = ColType.Text },
        ["१६.६ ऋण लिएको छ भने, के को लागि लिएको हो ?"] = new() { Table = "household", Column = "loan_purpose_summary", Type = ColType.Text },
        ["१६.६ ऋण लिएको छ भने, के को लागि लिएको हो ?/व्यवसाय"] = new() { Table = "household", Column = "loan_purpose_business", Type = ColType.SmallInt },
        ["१६.६ ऋण लिएको छ भने, के को लागि लिएको हो ?/घर घडेरी"] = new() { Table = "household", Column = "loan_purpose_house", Type = ColType.SmallInt },
        ["१६.६ ऋण लिएको छ भने, के को लागि लिएको हो ?/औषधि उपचार  खर्च"] = new() { Table = "household", Column = "loan_purpose_medical", Type = ColType.SmallInt },
        ["१६.६ ऋण लिएको छ भने, के को लागि लिएको हो ?/शिक्षा खर्च"] = new() { Table = "household", Column = "loan_purpose_education", Type = ColType.SmallInt },
        ["१६.६ ऋण लिएको छ भने, के को लागि लिएको हो ?/अन्य"] = new() { Table = "household", Column = "loan_purpose_other", Type = ColType.SmallInt },
        ["अन्य के का लागि हो खुलाउनु होला ।"] = new() { Table = "household", Column = "loan_purpose_other_detail", Type = ColType.Text },

        // ═══════════════════════════════════════════════════
        // Section 17-18: Housing & disaster
        // ═══════════════════════════════════════════════════
        ["१७ घर सम्बन्धी बिवरण"] = new() { Table = "household", Column = "house_info_note", Type = ColType.Ignore },
        ["१७. तपाईको कति वटा घर छन् ?"] = new() { Table = "household", Column = "house_count", Type = ColType.Double },
        ["१८.१ तपाई बसेको घर भूकम्प प्रतिरोधी छ ?"] = new() { Table = "household", Column = "earthquake_resistant", Type = ColType.Text },
        ["१८.२ तपाईको घर कुनै प्रकोपका कारण क्षति भएको हो ?"] = new() { Table = "household", Column = "disaster_damaged", Type = ColType.Text },
        ["१८.३ यदि हो भने अनुदान पाउनु भयो ?"] = new() { Table = "household", Column = "received_subsidy", Type = ColType.Text },
        ["१८.४ यदि पाउनु भएको भए कति किस्ता पाउनु भयो ?"] = new() { Table = "household", Column = "subsidy_installments", Type = ColType.Text },
        ["१८.५ यदि अनुदान पाउनु भएको भए तपाईको घर बनाउनु भयो त ?"] = new() { Table = "household", Column = "house_rebuilt", Type = ColType.Text },
        ["१८.६ तपाईको घर परिवारको जोखिम पारिवारिक योजना बनेको छ ?"] = new() { Table = "household", Column = "has_family_risk_plan", Type = ColType.Text },
        ["१८.७ तपाइको घर प्राकृतिक प्रकोपको जोखिममा छ ?"] = new() { Table = "household", Column = "disaster_risk", Type = ColType.Text },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?"] = new() { Table = "household", Column = "disaster_risk_summary", Type = ColType.Text },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/भूकम्प"] = new() { Table = "household", Column = "risk_earthquake", Type = ColType.SmallInt },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/बाढी"] = new() { Table = "household", Column = "risk_flood", Type = ColType.SmallInt },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/पहिरो"] = new() { Table = "household", Column = "risk_landslide", Type = ColType.SmallInt },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/चट्याङ्ग"] = new() { Table = "household", Column = "risk_lightning", Type = ColType.SmallInt },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/हावाहुरी"] = new() { Table = "household", Column = "risk_storm", Type = ColType.SmallInt },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/शितलहर"] = new() { Table = "household", Column = "risk_cold_wave", Type = ColType.SmallInt },
        ["१८.८ तपाइको घर कस्तो प्रकारको जोखिममा छ ?/डुवान वा कटान"] = new() { Table = "household", Column = "risk_inundation", Type = ColType.SmallInt },

        // ═══════════════════════════════════════════════════
        // Section 19-20: Photo & GPS
        // ═══════════════════════════════════════════════════
        ["१९. घर र उत्तरदाताको फोटो"] = new() { Table = "household", Column = "photo", Type = ColType.Text },
        ["१९. घर र उत्तरदाताको फोटो_URL"] = new() { Table = "household", Column = "photo_url", Type = ColType.Text },
        ["२०. घरको GPS (जियो कोड)"] = new() { Table = "household", Column = "geo_point", Type = ColType.Text },
        ["_२०. घरको GPS (जियो कोड)_latitude"] = new() { Table = "household", Column = "latitude", Type = ColType.Double },
        ["_२०. घरको GPS (जियो कोड)_longitude"] = new() { Table = "household", Column = "longitude", Type = ColType.Double },
        ["_२०. घरको GPS (जियो कोड)_altitude"] = new() { Table = "household", Column = "altitude", Type = ColType.Double },
        ["_२०. घरको GPS (जियो कोड)_precision"] = new() { Table = "household", Column = "gps_precision", Type = ColType.Double },

        // Closing & extras
        ["२१.धैर्यतापूर्वक तथ्याङ्क भर्नुभएकोमा धन्यवाद ।"] = new() { Table = "household", Column = "closing_note", Type = ColType.Ignore },
        ["२.५ नगरपालिका केन्द्र देखि यस टोल सम्मको दूरी_2"] = new() { Table = "household", Column = "distance_from_center_2", Type = ColType.Text },
        ["घ्यू कति लिटर बेच्\u200Dनु हुन्छ ?"] = new() { Table = "household", Column = "ghee_sold", Type = ColType.Double },
        ["घ्यू कति लिटर बेच्\u200Dनु हुन्छ ?_2"] = new() { Table = "household", Column = "ghee_sold_2", Type = ColType.Double },
        ["घ्यू कति लिटर बेच्\u200Dनु हुन्छ ?_3"] = new() { Table = "household", Column = "ghee_sold_3", Type = ColType.Double },

    }; // ← END of Map dictionary
} // ← END of HouseholdMapping class