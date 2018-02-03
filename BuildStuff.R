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

run <- function(type, file, timetitle, spacetitle, values, labels) {
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
  plotAllocs <- !is.null(spacetitle)
  
  listDf <- result %>% filter(Target_Method == "List")
  listDf$N <- sapply(listDf$Params, function(param) strtoi(gsub("N=", "", param), base = 10))
  listMeansDf <- group_by(listDf, N) %>% summarize(MeanTime=mean(Measurement_Value))
  if (plotAllocs) {
    listAllocDf <- group_by(listDf, N) %>% summarize(AllocatedBytes=Allocated_Bytes[1])
  }

  if (type == "Append") {  
    growthDf <- result %>% filter(Target_Method == "GrowthArray")
    growthDf$N <- sapply(growthDf$Params, function(param) strtoi(gsub("N=", "", param), base = 10))
    growthMeansDf <- group_by(growthDf, N) %>% summarize(MeanTime=mean(Measurement_Value))
    if (plotAllocs) {
      growthAllocDf <- group_by(growthDf, N) %>% summarize(AllocatedBytes=Allocated_Bytes[1])
    }
  } else {
    growthDf <- result %>% filter(Target_Method == "GrowthArray_O1")
    growthDf$N <- sapply(growthDf$Params, function(param) strtoi(gsub("N=", "", param), base = 10))
    growthMeansDf <- group_by(growthDf, N) %>% summarize(MeanTime=mean(Measurement_Value))
    growthDf2 <- result %>% filter(Target_Method == "GrowthArray_OLogN")
    growthDf2$N <- sapply(growthDf2$Params, function(param) strtoi(gsub("N=", "", param), base = 10))
    growthMeansDf2 <- group_by(growthDf2, N) %>% summarize(MeanTime=mean(Measurement_Value))
  }

  methods <- unique(result$Target_Method)
  if (type == "Append") {
    timelinePlot <- ggplot() +
      ggtitle(timetitle) +
      theme(plot.title = element_text(hjust = 0.5)) +
      xlab("N") +
      ylab(paste0("Average Time (", timeUnit, ")")) +
      scale_x_continuous(trans='log10', breaks=10^(1:10)) +
      scale_y_continuous(trans='log10', breaks=10^(1:10)) +
      geom_line(data=listMeansDf, aes(x=N, y=MeanTime,color="List")) +
      geom_point(data=listMeansDf, aes(x=N, y=MeanTime,color="List")) +
      geom_line(data=growthMeansDf, aes(x=N, y=MeanTime,color="GrowthArray")) +
      geom_point(data=growthMeansDf, aes(x=N, y=MeanTime,color="GrowthArray")) +
      scale_color_manual(name="Legend",
                        values=values,
                        breaks=methods,
                        labels=labels)
  } else {
    # We want to get the average time for 1 random access, not N of them
    listMeansDf$MeanTime <- listMeansDf$MeanTime / listMeansDf$N
    growthMeansDf$MeanTime <- growthMeansDf$MeanTime / growthMeansDf$N
    growthMeansDf2$MeanTime <- growthMeansDf2$MeanTime / growthMeansDf2$N

    timelinePlot <- ggplot() +
      ggtitle(timetitle) +
      theme(plot.title = element_text(hjust = 0.5)) +
      xlab("N") +
      ylab(paste0("Average Time (", timeUnit, ")")) +
      scale_x_continuous(trans='log10', breaks=10^(1:10)) +
      scale_y_continuous(trans='log10', breaks=10^(1:10)) +
      geom_line(data=listMeansDf, aes(x=N, y=MeanTime,color="List")) +
      geom_point(data=listMeansDf, aes(x=N, y=MeanTime,color="List")) +
      geom_line(data=growthMeansDf, aes(x=N, y=MeanTime,color="GrowthArray_O1")) +
      geom_point(data=growthMeansDf, aes(x=N, y=MeanTime,color="GrowthArray_O1")) +
      geom_line(data=growthMeansDf2, aes(x=N, y=MeanTime,color="GrowthArray_OLogN")) +
      geom_point(data=growthMeansDf2, aes(x=N, y=MeanTime,color="GrowthArray_OLogN")) +
      scale_color_manual(name="Legend",
                        values=values,
                        breaks=methods,
                        labels=labels)
  }

  printNice(timelinePlot)
  ggsaveNice(gsub("-measurements.csv", paste0("-timeline.png"), file), timelinePlot)

  if (plotAllocs) {
    allocationsPlot <- ggplot() +
      ggtitle(spacetitle) +
      theme(plot.title = element_text(hjust = 0.5)) +
      xlab("N") +
      ylab("Allocated Bytes") +
      scale_x_continuous(trans='log10', breaks=10^(1:10)) +
      scale_y_continuous(trans='log10', breaks=10^(1:10)) +
      geom_line(data=listAllocDf, aes(x=N, y=AllocatedBytes,color="List")) +
      geom_point(data=listAllocDf, aes(x=N, y=AllocatedBytes,color="List")) +
      geom_line(data=growthAllocDf, aes(x=N, y=AllocatedBytes,color="GrowthArray")) +
      geom_point(data=growthAllocDf, aes(x=N, y=AllocatedBytes,color="GrowthArray")) +
      scale_color_manual(name="Legend",
                        values=values,
                        breaks=methods,
                        labels=labels)
      
    printNice(allocationsPlot)
    ggsaveNice(gsub("-measurements.csv", paste0("-allocations.png"), file), allocationsPlot)
  }
}

#args <- commandArgs(trailingOnly = TRUE)
#files <- if (length(args) > 0) args else list.files()[list.files() %>% ends_with("-measurements.csv")]
#for (file in files) {
#}

run(
  type="Append",
  file="StsProject.Benchmarks.ListVsGrowthArray_Append-measurements.csv",
  timetitle="Average Time Needed to Append N Items",
  spacetitle="Space Needed to Append N Items",
  values=c("List"="red", "GrowthArray"="blue"),
  labels=c("List", "GrowthArray")
)

run(
  type="GetItem",
  file="StsProject.Benchmarks.ListVsGrowthArray_GetItem-measurements.csv",
  timetitle="Average Time Needed for Random Access, Size=N",
  spacetitle=NULL,
  values=c("List"="red", "GrowthArray_O1"="blue", "GrowthArray_OLogN"="green"),
  labels=c("List", "GrowthArray, O(1)", "GrowthArray, O(log N)")
)
