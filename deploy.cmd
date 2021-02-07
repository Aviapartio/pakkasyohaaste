gcloud builds submit --tag eu.gcr.io/adventti/pakkasyohaaste
gcloud alpha run deploy --image  eu.gcr.io/adventti/pakkasyohaaste --platform managed
