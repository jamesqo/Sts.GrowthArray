BenchmarkDotNetVersion <- "BenchmarkDotNet v0.10.12 "
dir.create(Sys.getenv("R_LIBS_USER"), recursive = TRUE, showWarnings = FALSE)
list.of.packages <- c("ggplot2", "dplyr", "gdata", "tidyr", "grid", "gridExtra", "Rcpp")
new.packages <- list.of.packages[!(list.of.packages %in% installed.packages()[,"Package"])]
if(length(new.packages)) install.packages(new.packages, lib = Sys.getenv("R_LIBS_USER"), repos = "http://cran.rstudio.com/")
library(ggplot2)
library(dplyr)
library(gdata)
library(tidyr)
library(grid)
library(gridExtra)

ends_with <- function(vars, match, ignore.case = TRUE) {
  if (ignore.case)
    match <- tolower(match)
  n <- nchar(match)

  if (ignore.case)
    vars <- tolower(vars)
  length <- nchar(vars)

  substr(vars, pmax(1, length - n + 1), length) == match
}
std.error <- function(x) sqrt(var(x)/length(x))
cummean <- function(x) cumsum(x)/(1:length(x))
BenchmarkDotNetVersionGrob <- textGrob(BenchmarkDotNetVersion, gp = gpar(fontface=3, fontsize=10), hjust=1, x=1)
nicePlot <- function(p) grid.arrange(p, bottom = BenchmarkDotNetVersionGrob)
printNice <- function(p) print(nicePlot(p))
ggsaveNice <- function(fileName, p, ...) {
  cat(paste0("*** Plot: ", fileName, " ***\n"))
  ggsave(fileName, plot = nicePlot(p), ...)
  cat("------------------------------\n")
}

args <- commandArgs(trailingOnly = TRUE)
files <- if (length(args) > 0) args else list.files()[list.files() %>% ends_with("-measurements.csv")]
for (file in files) {
  title <- gsub("-measurements.csv", "", basename(file))
  measurements <- read.csv(file, sep = ",")

  result <- measurements %>% filter(Measurement_IterationMode == "Result")
  if (nrow(result[is.na(result$Job_Id),]) > 0)
    result[is.na(result$Job_Id),]$Job_Id <- ""
  if (nrow(result[is.na(result$Params),]) > 0) {
    result[is.na(result$Params),]$Params <- ""
  } else {
    result$Job_Id <- trim(paste(result$Job_Id, result$Params))
  }
  result$Job_Id <- factor(result$Job_Id, levels = unique(result$Job_Id))

  timeUnit <- "ns"
  if (min(result$Measurement_Value) > 1000) {
    result$Measurement_Value <- result$Measurement_Value / 1000
    timeUnit <- "us"
  }
  if (min(result$Measurement_Value) > 1000) {
    result$Measurement_Value <- result$Measurement_Value / 1000
    timeUnit <- "ms"
  }
  if (min(result$Measurement_Value) > 1000) {
    result$Measurement_Value <- result$Measurement_Value / 1000
    timeUnit <- "sec"
  }

  resultStats <- result %>%
    group_by_(.dots = c("Target_Method", "Job_Id")) %>%
    summarise(se = std.error(Measurement_Value), Value = mean(Measurement_Value))

  listDf <- result %>% filter(Target_Method == "List")
  listDf$N <- sapply(listDf$Params, function(param) strtoi(gsub("N=", "", param), base = 10))
  listMeansDf <- group_by(listDf, N) %>% summarize(MeanTime=mean(Measurement_Value))
  
  growthDf <- result %>% filter(Target_Method == "GrowthArray")
  growthDf$N <- sapply(growthDf$Params, function(param) strtoi(gsub("N=", "", param), base = 10))
  growthMeansDf <- group_by(growthDf, N) %>% summarize(MeanTime=mean(Measurement_Value))

  timelinePlot <- ggplot() +
    xlab("N") +
    ylab(paste0("Average time to append N items (", timeUnit, ")")) +
    scale_x_continuous(trans='log10', breaks=10^(1:10)) +
    scale_y_continuous(trans='log10', breaks=10^(1:10)) +
    geom_line(data=listMeansDf, aes(x=N, y=MeanTime,color="List")) +
    geom_point(data=listMeansDf, aes(x=N, y=MeanTime,color="List")) +
    geom_line(data=growthMeansDf, aes(x=N, y=MeanTime,color="GrowthArray")) +
    geom_point(data=growthMeansDf, aes(x=N, y=MeanTime,color="GrowthArray")) +
    scale_color_manual(name="Legend", values=c("List"="red","GrowthArray"="blue")) +
    expand_limits(xlim=c(max(listMeansDf$N, growthMeansDf$N), max(listMeansDf$N, growthMeansDf$N)),
                  ylim=c(max(listMeansDf$MeanTime, growthMeansDf$MeanTime), max(listMeansDf$MeanTime, growthMeansDf$MeanTime)))

  printNice(timelinePlot)
  ggsaveNice(gsub("-measurements.csv", paste0("-timeline.png"), file), timelinePlot)
}
