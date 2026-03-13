import { Schema, model } from "mongoose";
import { IManuscript } from "@/interfaces/manuscript/IManuscript";

/**
 * Embedded sub-document schema for an individual chapter.
 * `timestamps: true` automatically manages `createdAt` / `updatedAt` fields
 * used by the Last-Write-Wins conflict detection in {@link Manuscript.service}.
 */
const ChapterSchema = new Schema(
  {
    chapterId: { type: String, required: true },
    title: { type: String, required: true, maxlength: 200 },
    content: { type: String, default: "" },
    order: { type: Number, required: true },
  },
  { timestamps: true },
);

/**
 * One `Manuscript` document exists per project.
 * `projectId` is the GUID issued by `server-core` and is used as the primary lookup key.
 */
const ManuscriptSchema = new Schema<IManuscript>(
  {
    projectId: { type: String, required: true, unique: true, index: true },
    chapters: [ChapterSchema],
  },
  { timestamps: true },
);

export const ManuscriptModel = model<IManuscript>("Manuscript", ManuscriptSchema);
