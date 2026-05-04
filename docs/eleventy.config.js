export default function (eleventyConfig) {
  eleventyConfig.addPassthroughCopy({ "./public/": "/" });

  return {
    dir: {
      input: ".",
      includes: "_includes",
      data: "_data",
      output: "_site",
    },
    templateFormats: ["njk", "html", "txt", "xml"],
    htmlTemplateEngine: "njk",
  };
}
