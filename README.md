# biobencher
This is a benchmark of bioinformatics and genomic analysis tools particularly MSA, phyllo tools using our base debigenic genomic and bioinformatic analysis pipeline

## Tools Benchmarked 
* BEDTools
* Clustalw2
* BLAST
* HMMER
* PHYLIP

* TODO: QuEST
* TODO: VELVET
* TODO: MUMMER

## We benchmarked performance of 5+ bioinformatic tools for MSA, Philo etc run on a debian linux machine pipeline.

### This benchmark setup currently works with my debigenic genomic and bioinformatics pipeline that runs on Ubuntu 22.04 , you can pull the docker image and clone this repo on the pipeline before you continue 

### How to Run

#### Step 1 - pull debigenic pipeline image from dockerhub
```
docker pull propenster/debigenic
```

#### Step 2 - Run the pipeline image
```
docker run -it propenster/debigenic bin/bash
```

#### clone this repo
```
git clone https://github.com/propenster/biobencher.git
```

#### cd into ./biobencher
```
cd ./biobencher
```

#### Run the Tool ./BioBencher
```
./BioBencher
```

#### N.B: You may need to make the app an executable first, you may do so by running the below command on linux
```
chmod +x ./BioBencher
```




