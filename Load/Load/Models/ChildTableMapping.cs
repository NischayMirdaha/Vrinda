namespace Load.Models;

public static class ChildTableMapping
{
    // ================================================================
    // FAMILY MEMBER — 77 keys, 43013 rows
    // ================================================================
    public static readonly Dictionary<string, ColumnMap> FamilyMap = new()
    {
        // Repeat metadata
        ["_index"] = new() { Table = "family_member", Column = "repeat_index", Type = ColType.Integer },
        ["_parent_table_name"] = new() { Table = "family_member", Column = "parent_table_name", Type = ColType.Text },
        ["_parent_index"] = new() { Table = "family_member", Column = "parent_index", Type = ColType.Integer },
        ["_submission__id"] = new() { Table = "family_member", Column = "submission_id", Type = ColType.BigInt },
        ["_submission__uuid"] = new() { Table = "family_member", Column = "submission_uuid", Type = ColType.Text },
        ["_submission__submission_time"] = new() { Table = "family_member", Column = "submission_time", Type = ColType.Timestamp },
        ["_submission__validation_status"] = new() { Table = "family_member", Column = "submission_validation", Type = ColType.Text },
        ["_submission__notes"] = new() { Table = "family_member", Column = "submission_notes", Type = ColType.Text },
        ["_submission__status"] = new() { Table = "family_member", Column = "submission_status", Type = ColType.Text },
        ["_submission__submitted_by"] = new() { Table = "family_member", Column = "submission_submitted_by", Type = ColType.Text },
        ["_submission___version__"] = new() { Table = "family_member", Column = "submission_version", Type = ColType.Text },
        ["_submission__tags"] = new() { Table = "family_member", Column = "submission_tags", Type = ColType.Text },

        // Section header
        ["<span style=\"color:blue\">घरका सदस्यहरुको जानकारी भर्दा घरमुलीको विवरणबाट भर्न शुरु गर्ने र जेठोबाट कान्छो सदस्य तिर घट्दो क्रममा भर्दै जानु होला । ध्यान दिनु होला कि कानूनीरुपमा अंश बण्डा भएको छैन वा सामाजिकरुपमा ति फरक घरका सदस्य हुन भन्\u200Dने गरि समाजमा पहिचान बनिसकेको छैन भने फरक घर वा फरक कोठामा बसे पनि सवै एउटै घरका सदस्यको रुपमा फर्म भर्नु पर्नेछ । </span>"] = new() { Table = "family_member", Column = "section_note", Type = ColType.Ignore },

        // 4.1-4.9: Personal info
        ["४.१. नाम"] = new() { Table = "family_member", Column = "full_name", Type = ColType.Text },
        ["४.२. ${Name} को थर के हो ?"] = new() { Table = "family_member", Column = "surname", Type = ColType.Text },
        ["४.३. ${Name} को जाति के हो ?"] = new() { Table = "family_member", Column = "caste", Type = ColType.Text },
        ["४.४. ${Name} को लिंग के हो ?"] = new() { Table = "family_member", Column = "gender", Type = ColType.Text },
        ["४.५. ${Name} र घरमुली बीचको नाता के हो ?"] = new() { Table = "family_member", Column = "relation_to_head", Type = ColType.Text },
        ["४.६. ${Name} को गणना गर्दाको उमेर ?"] = new() { Table = "family_member", Column = "age", Type = ColType.Double },
        ["४.७.  ${Name} ले बोल्ने भाषा के हो ?"] = new() { Table = "family_member", Column = "language", Type = ColType.Text },
        ["अन्य भए उल्लेख गर्नु होस ।"] = new() { Table = "family_member", Column = "language_other", Type = ColType.Text },
        ["४.८. ${Name}  धर्म के हो ?"] = new() { Table = "family_member", Column = "religion", Type = ColType.Text },
        ["अन्य धर्म भए उल्लेख गर्नुहोस"] = new() { Table = "family_member", Column = "religion_other", Type = ColType.Text },
        ["४.९. ${Name}  को बैवाहिक स्थिति"] = new() { Table = "family_member", Column = "marital_status", Type = ColType.Text },

        // 4.10-4.12: Education
        ["४.१०.  ${Name}  को शैक्षिक योग्यता"] = new() { Table = "family_member", Column = "education_level", Type = ColType.Text },
        ["खुलाउनु होला ।"] = new() { Table = "family_member", Column = "education_detail", Type = ColType.Text },
        ["४.११ ${Name} को साक्षरताको स्थिति के हो ?"] = new() { Table = "family_member", Column = "literacy_status", Type = ColType.Text },
        ["४.१२. ${Name}  कुनै क्षेत्रको विशेषज्ञ हो ?"] = new() { Table = "family_member", Column = "expertise_area", Type = ColType.Text },
        ["४.१२.१ अन्य केहो खुलाउने ।"] = new() { Table = "family_member", Column = "expertise_other", Type = ColType.Text },

        // 4.13: Occupation
        ["४.१३. ${Name}  को पेशा के हो ?"] = new() { Table = "family_member", Column = "occupation", Type = ColType.Text },
        ["अन्य पेशा भए उल्लेख गर्ने ।"] = new() { Table = "family_member", Column = "occupation_other", Type = ColType.Text },
        ["४.१३.१ ${Name} को सरकारी जागिरको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "govt_job_type", Type = ColType.Text },
        ["४.१३.२ ${Name} को स्वरोजगारको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "self_employment_type", Type = ColType.Text },
        ["४.१३.२.१ ${Name} को  सीपमा स्वरोजगारको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "self_employment_skill", Type = ColType.Text },
        ["४.१३.२.२ ${Name} को  कृषिमा स्वरोजगारको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "self_employment_agri", Type = ColType.Text },
        ["४.१३.२.३ ${Name} को  व्यवसायिक स्वरोजगारको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "self_employment_business", Type = ColType.Text },
        ["४.१३.२.४ ${Name} को  सेवामा आधारित स्वरोजगारको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "self_employment_service", Type = ColType.Text },
        ["४.१३.२.५ ${Name} को  घरमै बसेर गरिने स्वरोजगारको विशिष्टिकरण गर्नु होस ।"] = new() { Table = "family_member", Column = "self_employment_home", Type = ColType.Text },

        // 4.14-4.15: Training
        ["४.१४. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?"] = new() { Table = "family_member", Column = "has_vocational_training", Type = ColType.Text },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?"] = new() { Table = "family_member", Column = "training_summary", Type = ColType.Text },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/सिलाई ,बुनाई, बुटिक, सृङगार, पार्लर आदि"] = new() { Table = "family_member", Column = "training_sewing", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/निर्माण सम्बन्धी (राज मिस्त्रि,  आदी)"] = new() { Table = "family_member", Column = "training_construction", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/सिकर्मी कार्पेन्ट्री"] = new() { Table = "family_member", Column = "training_carpentry", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/डकर्मी"] = new() { Table = "family_member", Column = "training_metalwork", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/बिजुली मिस्त्री"] = new() { Table = "family_member", Column = "training_electrical", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/धारा पाईप पलाम्बिंग"] = new() { Table = "family_member", Column = "training_plumbing", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/अटोमोबाइल र मेकानिक्स"] = new() { Table = "family_member", Column = "training_automobile", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/कृषि सम्बन्धी (जेटी, जेटिए र खाद्य प्रशाेधन आदि)"] = new() { Table = "family_member", Column = "training_agriculture", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/जनस्वास्थ्य सम्बन्धी"] = new() { Table = "family_member", Column = "training_public_health", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/पशुस्वास्थ्य  सम्बन्धी"] = new() { Table = "family_member", Column = "training_animal_health", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/पर्यटन, टुर गाइड, ट्राभल र सत्कार"] = new() { Table = "family_member", Column = "training_tourism", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/कला सम्बन्धी"] = new() { Table = "family_member", Column = "training_arts", Type = ColType.SmallInt },
        ["४.१५. ${Name} ले कुनै व्यवसायिक तालिमको लिनु भएको छ ?/अन्य"] = new() { Table = "family_member", Column = "training_other", Type = ColType.SmallInt },
        ["अन्य व्यवसायिक तालिमको भए उल्लेख गर्नुस"] = new() { Table = "family_member", Column = "training_other_detail", Type = ColType.Text },

        // 4.16-4.18: Residence
        ["४.१६. ${Name}  को बसोबासको अवस्था"] = new() { Table = "family_member", Column = "residence_status", Type = ColType.Text },
        ["४.१७. बिदेशमा भए ${Name} कुन देशमा हुनुहुन्छ ?"] = new() { Table = "family_member", Column = "abroad_country", Type = ColType.Text },
        ["४.१८. ${Name} बिदेशमा के का लागि बसोबास गर्दै हुनुहुन्छ ?"] = new() { Table = "family_member", Column = "abroad_purpose", Type = ColType.Text },

        // 4.19-4.21: Health
        ["४.१९.  ${Name} को अपांगता स्थिति"] = new() { Table = "family_member", Column = "disability_status", Type = ColType.Text },
        ["४.२०. ${Name} को स्वास्थ्य स्थिति"] = new() { Table = "family_member", Column = "health_status", Type = ColType.Text },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?"] = new() { Table = "family_member", Column = "disease_summary", Type = ColType.Text },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/मुटु रोग"] = new() { Table = "family_member", Column = "disease_heart", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/क्यान्सर"] = new() { Table = "family_member", Column = "disease_cancer", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/सुगर वा चिनि रोग"] = new() { Table = "family_member", Column = "disease_diabetes", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/बाथ रोग"] = new() { Table = "family_member", Column = "disease_arthritis", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/नशा च्यापिएको"] = new() { Table = "family_member", Column = "disease_addiction", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/एचआइभि र एड्स"] = new() { Table = "family_member", Column = "disease_hiv_aids", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/दम"] = new() { Table = "family_member", Column = "disease_asthma", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/किड्नी"] = new() { Table = "family_member", Column = "disease_kidney", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/प्यारालाइसिस"] = new() { Table = "family_member", Column = "disease_paralysis", Type = ColType.SmallInt },
        ["४.२१. ${Name} लाई कुन रोग लागेको छ ?/स्पाइनल इन्जुरी"] = new() { Table = "family_member", Column = "disease_spinal_injury", Type = ColType.SmallInt },

        // 4.22-4.23: Insurance
        ["४.२२.  ${Name} ले सामाजिक सुरक्षा भत्ता लिए नलिएको अवस्था"] = new() { Table = "family_member", Column = "social_security_status", Type = ColType.Text },
        ["४.२३  ${Name} को विमा गरेको छ कि छैन ?"] = new() { Table = "family_member", Column = "has_insurance", Type = ColType.Text },
        ["४.२३.१ कुन कुन विमा गरेको छ ?"] = new() { Table = "family_member", Column = "insurance_summary", Type = ColType.Text },
        ["४.२३.१ कुन कुन विमा गरेको छ ?/जीवन विमा"] = new() { Table = "family_member", Column = "insurance_life", Type = ColType.SmallInt },
        ["४.२३.१ कुन कुन विमा गरेको छ ?/स्वास्थ्य विमा"] = new() { Table = "family_member", Column = "insurance_health", Type = ColType.SmallInt },
        ["४.२३.१ कुन कुन विमा गरेको छ ?/अन्य"] = new() { Table = "family_member", Column = "insurance_other", Type = ColType.SmallInt },
        ["अन्य भए खुलाउनुस ।"] = new() { Table = "family_member", Column = "insurance_other_detail", Type = ColType.Text },
    };

// ════════════════════════════════════════════════════
// CONTINUED IN PART 7B...
// ════════════════════════════════════════════════════
        // ================================================================
    // HOUSE DETAIL — 22 keys, 8647 rows
    // ================================================================
    public static readonly Dictionary<string, ColumnMap> HouseDetailMap = new()
    {
        // Repeat metadata
        ["_index"] = new() { Table = "house_detail", Column = "repeat_index", Type = ColType.Integer },
        ["_parent_table_name"] = new() { Table = "house_detail", Column = "parent_table_name", Type = ColType.Text },
        ["_parent_index"] = new() { Table = "house_detail", Column = "parent_index", Type = ColType.Integer },
        ["_submission__id"] = new() { Table = "house_detail", Column = "submission_id", Type = ColType.BigInt },
        ["_submission__uuid"] = new() { Table = "house_detail", Column = "submission_uuid", Type = ColType.Text },
        ["_submission__submission_time"] = new() { Table = "house_detail", Column = "submission_time", Type = ColType.Timestamp },
        ["_submission__validation_status"] = new() { Table = "house_detail", Column = "submission_validation", Type = ColType.Text },
        ["_submission__notes"] = new() { Table = "house_detail", Column = "submission_notes", Type = ColType.Text },
        ["_submission__status"] = new() { Table = "house_detail", Column = "submission_status", Type = ColType.Text },
        ["_submission__submitted_by"] = new() { Table = "house_detail", Column = "submission_submitted_by", Type = ColType.Text },
        ["_submission___version__"] = new() { Table = "house_detail", Column = "submission_version", Type = ColType.Text },
        ["_submission__tags"] = new() { Table = "house_detail", Column = "submission_tags", Type = ColType.Text },

        // Section 17: House details
        ["१७.१.  तपाई हाल बसेको घरको स्वामित्व कस्तो प्रकारको हो ?"] = new() { Table = "house_detail", Column = "ownership_type", Type = ColType.Text },
        ["१७.२. तपाईको घरमा जम्मा कति वटा कोठा छन् ?"] = new() { Table = "house_detail", Column = "total_rooms", Type = ColType.Double },
        ["१७.३  कति कोठा भाडामा दिनु भएको छ ?"] = new() { Table = "house_detail", Column = "rooms_rented", Type = ColType.Double },
        ["१७.४  तपाई बसोबास गर्ने घरको छानाको प्रकार कस्ताे छ ?"] = new() { Table = "house_detail", Column = "roof_type", Type = ColType.Text },
        ["अन्य भनेको खुलाउनु होला ।"] = new() { Table = "house_detail", Column = "roof_other", Type = ColType.Text },
        ["१७.५  छानाको प्रकार अन्य भए उल्लेख गर्नुहोस्"] = new() { Table = "house_detail", Column = "roof_other_detail", Type = ColType.Text },
        ["१७.६  तपाई बसेको घरको जग कस्तो प्रकारको छ ?"] = new() { Table = "house_detail", Column = "foundation_type", Type = ColType.Text },
        ["१७.७  घरको जग अन्य भए उल्लेख गर्नुहोस् ।"] = new() { Table = "house_detail", Column = "foundation_other", Type = ColType.Text },
        ["१७.८  के तँपाईले घरनक्सा पास (अभिलेखिकरण) गर्नु भएको छ ?"] = new() { Table = "house_detail", Column = "has_building_permit", Type = ColType.Text },
        ["१७.९  तपाई बसेको घर भवन मापदण्ड अनुसार बनेको छ ?"] = new() { Table = "house_detail", Column = "meets_building_code", Type = ColType.Text },
    };

    // ================================================================
    // HH DEATH RECORD — 53 keys, 118 rows
    // ================================================================
    public static readonly Dictionary<string, ColumnMap> DeathRecordMap = new()
    {
        // Repeat metadata
        ["_index"] = new() { Table = "hh_death_record", Column = "repeat_index", Type = ColType.Integer },
        ["_parent_table_name"] = new() { Table = "hh_death_record", Column = "parent_table_name", Type = ColType.Text },
        ["_parent_index"] = new() { Table = "hh_death_record", Column = "parent_index", Type = ColType.Integer },
        ["_submission__id"] = new() { Table = "hh_death_record", Column = "submission_id", Type = ColType.BigInt },
        ["_submission__uuid"] = new() { Table = "hh_death_record", Column = "submission_uuid", Type = ColType.Text },
        ["_submission__submission_time"] = new() { Table = "hh_death_record", Column = "submission_time", Type = ColType.Timestamp },
        ["_submission__validation_status"] = new() { Table = "hh_death_record", Column = "submission_validation", Type = ColType.Text },
        ["_submission__notes"] = new() { Table = "hh_death_record", Column = "submission_notes", Type = ColType.Text },
        ["_submission__status"] = new() { Table = "hh_death_record", Column = "submission_status", Type = ColType.Text },
        ["_submission__submitted_by"] = new() { Table = "hh_death_record", Column = "submission_submitted_by", Type = ColType.Text },
        ["_submission___version__"] = new() { Table = "hh_death_record", Column = "submission_version", Type = ColType.Text },
        ["_submission__tags"] = new() { Table = "hh_death_record", Column = "submission_tags", Type = ColType.Text },

        // 5.2: Deceased info
        ["५.२.१. मृत्यु हुनेको नाम लेख्नुहोस"] = new() { Table = "hh_death_record", Column = "deceased_name", Type = ColType.Text },
        ["५.२.२. मृत्यु हुनेको लिंग छान्नुहोस"] = new() { Table = "hh_death_record", Column = "deceased_gender", Type = ColType.Text },
        ["५.२.३. मृत्युहुनेको उमेर कति थियो ?"] = new() { Table = "hh_death_record", Column = "deceased_age_group", Type = ColType.Text },
        ["५.२.३.१ मृत्यू हुदा कति वर्षको हुनुहुन्थ्यो ?"] = new() { Table = "hh_death_record", Column = "deceased_age_detail", Type = ColType.Double },
        ["५.२.३. मृत्युहुनेको उमेर कति थियो ? (वर्षमा लेख्नुहोस)"] = new() { Table = "hh_death_record", Column = "deceased_age_years", Type = ColType.Text },

        // Cause of death (multi-select)
        ["५.२.४. मृत्युको कारण के थियो ?"] = new() { Table = "hh_death_record", Column = "cause_summary", Type = ColType.Text },
        ["५.२.४. मृत्युको कारण के थियो ?/संक्रामक रोगहरु"] = new() { Table = "hh_death_record", Column = "cause_communicable", Type = ColType.SmallInt },
        ["५.२.४. मृत्युको कारण के थियो ?/गैर संक्रामक रोगहरु"] = new() { Table = "hh_death_record", Column = "cause_non_communicable", Type = ColType.SmallInt },
        ["५.२.४. मृत्युको कारण के थियो ?/जीवनशैली जन्य रोगहरु"] = new() { Table = "hh_death_record", Column = "cause_lifestyle", Type = ColType.SmallInt },
        ["५.२.४. मृत्युको कारण के थियो ?/मृत्यूका अन्य कारणहरु"] = new() { Table = "hh_death_record", Column = "cause_other", Type = ColType.SmallInt },

        // Communicable diseases (multi-select)
        ["५.२.४.१ कुन संक्रामक रोगले हो ?"] = new() { Table = "hh_death_record", Column = "communicable_summary", Type = ColType.Text },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/न्युमोनिया"] = new() { Table = "hh_death_record", Column = "communicable_pneumonia", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/क्षयरोग / टी.बी."] = new() { Table = "hh_death_record", Column = "communicable_tb", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/एच.आइ.भी./एड्स"] = new() { Table = "hh_death_record", Column = "communicable_hiv_aids", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/हेपाटाइटिस B र C"] = new() { Table = "hh_death_record", Column = "communicable_hepatitis", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/डेंगु"] = new() { Table = "hh_death_record", Column = "communicable_dengue", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/कोभिड–१९ (COVID-19)"] = new() { Table = "hh_death_record", Column = "communicable_covid19", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/मलेरिया"] = new() { Table = "hh_death_record", Column = "communicable_malaria", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/टाइफाइड"] = new() { Table = "hh_death_record", Column = "communicable_typhoid", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/डायरिया/झाडा"] = new() { Table = "hh_death_record", Column = "communicable_diarrhea", Type = ColType.SmallInt },
        ["५.२.४.१ कुन संक्रामक रोगले हो ?/मेनेन्जाइटिस"] = new() { Table = "hh_death_record", Column = "communicable_meningitis", Type = ColType.SmallInt },

        // Non-communicable diseases (multi-select)
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?"] = new() { Table = "hh_death_record", Column = "noncommunicable_summary", Type = ColType.Text },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/मुटु रोग"] = new() { Table = "hh_death_record", Column = "noncommunicable_heart", Type = ColType.SmallInt },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/क्यानसर"] = new() { Table = "hh_death_record", Column = "noncommunicable_cancer", Type = ColType.SmallInt },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/मधुमेह"] = new() { Table = "hh_death_record", Column = "noncommunicable_diabetes", Type = ColType.SmallInt },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/स्वासप्रश्वासका रोग"] = new() { Table = "hh_death_record", Column = "noncommunicable_respiratory", Type = ColType.SmallInt },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/मृगौलाको समस्या"] = new() { Table = "hh_death_record", Column = "noncommunicable_kidney", Type = ColType.SmallInt },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/उच्च रक्तचाप"] = new() { Table = "hh_death_record", Column = "noncommunicable_hypertension", Type = ColType.SmallInt },
        ["५.२.४.२ कुन गैरसंक्रामक रोगले हो ?/पार्किन्सन/अल्जाइमर्स/डिमेन्सिया"] = new() { Table = "hh_death_record", Column = "noncommunicable_dementia", Type = ColType.SmallInt },

        // Lifestyle causes (multi-select)
        ["५.२.४.३ खराव जीवनशैलीका कारण"] = new() { Table = "hh_death_record", Column = "lifestyle_summary", Type = ColType.Text },
        ["५.२.४.३ खराव जीवनशैलीका कारण/अत्यधिक मोटोपन"] = new() { Table = "hh_death_record", Column = "lifestyle_obesity", Type = ColType.SmallInt },
        ["५.२.४.३ खराव जीवनशैलीका कारण/तनाव र मानसिक रोग"] = new() { Table = "hh_death_record", Column = "lifestyle_mental_stress", Type = ColType.SmallInt },
        ["५.२.४.३ खराव जीवनशैलीका कारण/मध्यपानका कारण"] = new() { Table = "hh_death_record", Column = "lifestyle_alcohol", Type = ColType.SmallInt },
        ["५.२.४.३ खराव जीवनशैलीका कारण/धूम्रपान र सुर्ती सेवनजन्य रोगहरू"] = new() { Table = "hh_death_record", Column = "lifestyle_smoking", Type = ColType.SmallInt },

        // Other causes (multi-select)
        ["५.२.४.५ अन्य कारणहरु"] = new() { Table = "hh_death_record", Column = "other_cause_summary", Type = ColType.Text },
        ["५.२.४.५ अन्य कारणहरु/सडक दुर्घटना"] = new() { Table = "hh_death_record", Column = "other_road_accident", Type = ColType.SmallInt },
        ["५.२.४.५ अन्य कारणहरु/अचानक हृदयघात"] = new() { Table = "hh_death_record", Column = "other_heart_attack", Type = ColType.SmallInt },
        ["५.२.४.५ अन्य कारणहरु/आत्महत्या"] = new() { Table = "hh_death_record", Column = "other_suicide", Type = ColType.SmallInt },
        ["५.२.४.५ अन्य कारणहरु/सर्पदंश, जनावरको टोका"] = new() { Table = "hh_death_record", Column = "other_animal_bite", Type = ColType.SmallInt },
        ["५.२.४.५ अन्य कारणहरु/प्राकृतिक प्रकोप"] = new() { Table = "hh_death_record", Column = "other_natural_disaster", Type = ColType.SmallInt },
        ["५.२.४.५ अन्य कारणहरु/गर्भावस्था विग्रिएर"] = new() { Table = "hh_death_record", Column = "other_pregnancy", Type = ColType.SmallInt },
    };

} // ← END of ChildTableMapping class