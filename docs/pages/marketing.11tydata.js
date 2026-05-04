export default {
  eleventyComputed: {
    title: (data) => data.variant?.title,
    description: (data) => data.variant?.description,
    keywords: (data) => data.variant?.keywords,
    h1: (data) => data.variant?.h1,
    pitch: (data) => data.variant?.pitch,
  },
};
